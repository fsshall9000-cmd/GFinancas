using GFinancas.Application.DTOs.Aplicacoes;
using GFinancas.Application.Exceptions;
using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Application.Interfaces.Repository;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Application.Interfaces.Services;
using GFinancas.Domain.Entities;
using GFinancas.Domain.Enums;

namespace GFinancas.Application.Services;

/// <summary>
/// Valida aportes e resgates e coordena a gravação da movimentação,
/// a atualização do valor atual da aplicação e seu histórico.
/// </summary>
public class MovimentacaoAplicacaoService
    : IMovimentacaoAplicacaoService
{
    private readonly IMovimentacaoAplicacaoRepository
        _movimentacaoRepository;

    private readonly IAplicacaoFinanceiraRepository
        _aplicacaoRepository;

    private readonly IContaRepository
        _contaRepository;

    private readonly IUsuarioAtualService
        _usuarioAtualService;

    private readonly ISaldoContaService
        _saldoContaService;

    private readonly IHistoricoValorAplicacaoRepository
        _historicoValorAplicacaoRepository;

    private readonly IUnitOfWork
        _unitOfWork;


    /// <summary>
    /// Recebe as dependências utilizadas pelo serviço.
    /// </summary>
    public MovimentacaoAplicacaoService(
        IMovimentacaoAplicacaoRepository movimentacaoRepository,
        IAplicacaoFinanceiraRepository aplicacaoRepository,
        IContaRepository contaRepository,
        IUsuarioAtualService usuarioAtualService,
        ISaldoContaService saldoContaService,
        IHistoricoValorAplicacaoRepository historicoValorAplicacaoRepository,
        IUnitOfWork unitOfWork)
    {
        _movimentacaoRepository =
            movimentacaoRepository;

        _aplicacaoRepository =
            aplicacaoRepository;

        _contaRepository =
            contaRepository;

        _usuarioAtualService =
            usuarioAtualService;

        _saldoContaService =
            saldoContaService;

        _historicoValorAplicacaoRepository =
            historicoValorAplicacaoRepository;

        _unitOfWork =
            unitOfWork;
    }


    /// <summary>
    /// Registra um aporte ou resgate em uma aplicação financeira.
    /// </summary>
    public async Task<MovimentacaoAplicacaoResponseDto>
        RegistrarAsync(
            CriarMovimentacaoAplicacaoDto dto)
    {
        // ------------------------------------------------
        // 1. Identifica o usuário autenticado
        // ------------------------------------------------

        var usuarioId =
            _usuarioAtualService.UsuarioId;


        // ------------------------------------------------
        // 2. Valida os dados básicos do DTO
        // ------------------------------------------------

        ValidarDto(dto);


        // ------------------------------------------------
        // 3. Busca a aplicação do usuário
        // ------------------------------------------------

        var aplicacao =
            await _aplicacaoRepository
                .ObterPorIdAsync(
                    dto.AplicacaoFinanceiraId,
                    usuarioId);


        if (aplicacao is null)
        {
            throw new NaoEncontradoException(
                "Aplicação financeira não encontrada.");
        }


        // ------------------------------------------------
        // 4. Verifica se a aplicação está ativa
        // ------------------------------------------------

        if (!aplicacao.Ativa)
        {
            throw new ValidacaoException(
                "Não é possível movimentar uma aplicação desativada.");
        }


        // ------------------------------------------------
        // 5. Busca a conta
        // ------------------------------------------------

        var conta =
            await _contaRepository
                .ObterPorIdAsync(
                    dto.ContaId);


        // ------------------------------------------------
        // 6. Verifica se a conta pertence ao usuário
        // ------------------------------------------------

        if (conta is null ||
            conta.UsuarioId != usuarioId)
        {
            throw new NaoEncontradoException(
                "Conta não encontrada.");
        }


        // ------------------------------------------------
        // 7. Valida a data da movimentação
        //
        // A movimentação:
        //
        // - não pode estar no futuro;
        // - não pode ser anterior ao início da aplicação;
        // - não pode ser anterior à criação da conta.
        // ------------------------------------------------

        ValidarDataMovimentacao(
            dto.Data,
            aplicacao,
            conta);


        // ------------------------------------------------
        // 8. Regra específica para APORTE
        // ------------------------------------------------

        if (dto.Tipo ==
            TipoMovimentacaoAplicacao.Aporte)
        {
            await ValidarAporteAsync(
                conta.Id,
                dto.Valor);
        }


        // ------------------------------------------------
        // 9. Regra específica para RESGATE
        // ------------------------------------------------

        if (dto.Tipo == TipoMovimentacaoAplicacao.Resgate)
        {
            await ValidarResgateHistoricoAsync(
            aplicacao,
            usuarioId,
            dto.Data,
            dto.Valor);

            await ValidarSaldosPosterioresAsync(
            aplicacao,
            dto.Data,
            dto.Valor);

            await ValidarSequenciaCronologicaAsync(
            aplicacao,
            usuarioId,
            dto.Data,
            dto.Valor);
    }


        // ------------------------------------------------
        // 10. Monta a movimentação
        // ------------------------------------------------

        var movimentacao =
            new MovimentacaoAplicacao
            {
                AplicacaoFinanceiraId =
                    aplicacao.Id,

                ContaId =
                    conta.Id,

                Valor =
                    dto.Valor,

                Data =
                    dto.Data,

                Tipo =
                    dto.Tipo
            };


        // ------------------------------------------------
        // 11. TRANSAÇÃO ATÔMICA
        //
        // As operações abaixo devem acontecer juntas:
        //
        // 1 - registrar movimentação;
        // 2 - atualizar histórico;
        // 3 - atualizar valor atual quando necessário.
        //
        // Se qualquer operação falhar, ocorre rollback.
        // ------------------------------------------------

        await _unitOfWork.ExecutarEmTransacaoAsync(
            async () =>
            {
                // ----------------------------------------
                // 11.1 Registra a movimentação
                // ----------------------------------------

                await _movimentacaoRepository
                    .AdicionarAsync(
                        movimentacao);


                // ----------------------------------------
                // 11.2 Define o impacto patrimonial
                //
                // Aporte:
                // + valor
                //
                // Resgate:
                // - valor
                // ----------------------------------------

                var impacto =
                    dto.Tipo ==
                    TipoMovimentacaoAplicacao.Aporte
                        ? dto.Valor
                        : -dto.Valor;


                // ----------------------------------------
                // 11.3 Obtém a data usada pelo histórico
                // ----------------------------------------

                var dataReferencia =
                    DateOnly.FromDateTime(
                        dto.Data);


                // ----------------------------------------
                // 11.4 Busca snapshots existentes
                // a partir da data da movimentação.
                //
                // A lista foi obtida ANTES de eventual
                // criação do snapshot desta movimentação.
                // ----------------------------------------

                var historicosPosteriores =
                    (await _historicoValorAplicacaoRepository
                        .ListarAPartirDaDataAsync(
                            aplicacao.Id,
                            dataReferencia))
                    .OrderBy(h => h.DataReferencia)
                    .ToList();


                // ----------------------------------------
                // 11.5 Procura snapshot da própria data
                // ----------------------------------------

                var historicoDaData =
                    historicosPosteriores
                        .FirstOrDefault(h =>
                            h.DataReferencia ==
                            dataReferencia);


                // ----------------------------------------
                // 11.6 Descobre se existe uma atualização
                // explícita de mercado na própria data
                // ou posteriormente.
                //
                // AtualizacaoMercado representa uma nova
                // referência absoluta do valor da
                // aplicação.
                // ----------------------------------------

                var existeAtualizacaoMercadoPosteriorOuNaData =
                    historicosPosteriores
                        .Any(h =>
                            h.DataReferencia >=
                                dataReferencia &&
                            h.Origem ==
                                OrigemHistoricoAplicacao
                                    .AtualizacaoMercado);


                // ----------------------------------------
                // 11.7 Se ainda não existir histórico
                // naquela data, cria um snapshot
                // originado pela movimentação.
                // ----------------------------------------

                if (historicoDaData is null)
                {
                    var historicoAnterior =
                        await _historicoValorAplicacaoRepository
                            .ObterUltimoAntesDaDataAsync(
                                aplicacao.Id,
                                dataReferencia);


                    var valorBase =
                        historicoAnterior?.Valor
                        ?? 0m;


                    historicoDaData =
                        new HistoricoValorAplicacao
                        {
                            AplicacaoFinanceiraId =
                                aplicacao.Id,

                            DataReferencia =
                                dataReferencia,

                            Valor =
                                valorBase + impacto,

                            Origem =
                                OrigemHistoricoAplicacao
                                    .Movimentacao
                        };


                    await _historicoValorAplicacaoRepository
                        .AdicionarAsync(
                            historicoDaData);
                }


                // ----------------------------------------
                // 11.8 Propaga o impacto.
                //
                // Somente snapshots automáticos de
                // movimentação podem ser recalculados.
                //
                // Ao encontrar AtualizacaoMercado,
                // interrompemos a propagação, pois aquele
                // valor passa a ser uma nova referência
                // absoluta.
                //
                // REGRA DA MESMA DATA:
                //
                // Se já existe AtualizacaoMercado na
                // própria data, ela prevalece como valor
                // de fechamento/referência daquele dia.
                // ----------------------------------------

                foreach (var historico
                         in historicosPosteriores)
                {
                    if (historico.Origem ==
                        OrigemHistoricoAplicacao
                            .AtualizacaoMercado)
                    {
                        break;
                    }


                    historico.Valor +=
                        impacto;


                    await _historicoValorAplicacaoRepository
                        .AtualizarAsync(
                            historico);
                }


                // ----------------------------------------
                // 11.9 Atualiza ValorAtual.
                //
                // Se existe uma AtualizacaoMercado na
                // data ou depois dela, NÃO propagamos o
                // impacto até o valor atual.
                //
                // A atualização explícita de mercado já
                // representa uma posição posterior mais
                // confiável da aplicação.
                // ----------------------------------------

                if (!existeAtualizacaoMercadoPosteriorOuNaData)
                {
                    aplicacao.ValorAtual +=
                        impacto;


                    await _aplicacaoRepository
                        .AtualizarAsync(
                            aplicacao);
                }
            });


        // ------------------------------------------------
        // 12. Retorna a movimentação criada
        // ------------------------------------------------

        return Mapear(
            movimentacao);
    }


    /// <summary>
    /// Lista as movimentações de uma aplicação pertencente
    /// ao usuário autenticado.
    /// </summary>
    public async Task<
        IEnumerable<MovimentacaoAplicacaoResponseDto>>
        ListarPorAplicacaoAsync(
            int aplicacaoId)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;


        // Verifica primeiro se a aplicação pertence
        // ao usuário autenticado.

        var aplicacao =
            await _aplicacaoRepository
                .ObterPorIdAsync(
                    aplicacaoId,
                    usuarioId);


        if (aplicacao is null)
        {
            throw new NaoEncontradoException(
                "Aplicação financeira não encontrada.");
        }


        var movimentacoes =
            await _movimentacaoRepository
                .ListarPorAplicacaoAsync(
                    aplicacaoId,
                    usuarioId);


        return movimentacoes
            .Select(Mapear)
            .ToList();
    }


    // ====================================================
    // VALIDAÇÕES BÁSICAS
    // ====================================================

    /// <summary>
    /// Valida os dados básicos recebidos para a movimentação.
    /// </summary>
    private static void ValidarDto(
        CriarMovimentacaoAplicacaoDto dto)
    {
        if (dto.Valor <= 0)
        {
            throw new ValidacaoException(
                "O valor da movimentação deve ser maior que zero.");
        }


        if (dto.Data == default)
        {
            throw new ValidacaoException(
                "A data da movimentação é obrigatória.");
        }


        if (!Enum.IsDefined(
                typeof(TipoMovimentacaoAplicacao),
                dto.Tipo))
        {
            throw new ValidacaoException(
                "O tipo da movimentação é inválido.");
        }
    }


    // ====================================================
    // VALIDAÇÃO DA DATA
    // ====================================================

    /// <summary>
    /// Valida a consistência temporal da movimentação.
    /// </summary>
    private static void ValidarDataMovimentacao(
        DateTime dataMovimentacao,
        AplicacaoFinanceira aplicacao,
        Conta conta)
    {
        var dataMovimentacaoDate =
            DateOnly.FromDateTime(
                dataMovimentacao);


        var hoje =
            DateOnly.FromDateTime(
                DateTime.Today);


        var dataInicioAplicacao =
            DateOnly.FromDateTime(
                aplicacao.DataInicio);


        // ------------------------------------------------
        // Não permite movimentação futura
        // ------------------------------------------------

        if (dataMovimentacaoDate >
            hoje)
        {
            throw new ValidacaoException(
                "A data da movimentação não pode ser futura.");
        }


        // ------------------------------------------------
        // Não permite movimentação anterior à aplicação
        // ------------------------------------------------

        if (dataMovimentacaoDate <
            dataInicioAplicacao)
        {
            throw new ValidacaoException(
                "A movimentação não pode ocorrer antes do início da aplicação.");
        }


        // ------------------------------------------------
        // Não permite movimentação anterior à conta
        // ------------------------------------------------

        if (dataMovimentacaoDate <
            conta.DataCriacao)
        {
            throw new ValidacaoException(
                "A movimentação não pode ocorrer antes da criação da conta.");
        }
    }


    // ====================================================
    // VALIDAÇÃO DO APORTE
    // ====================================================

    /// <summary>
    /// Rejeita aportes cujo valor excede o saldo disponível
    /// na conta.
    /// </summary>
    private async Task ValidarAporteAsync(
        int contaId,
        decimal valor)
    {
        var saldoAtual =
            await _saldoContaService
                .CalcularAsync(
                    contaId);


        if (valor >
            saldoAtual)
        {
            throw new ValidacaoException(
                "Saldo insuficiente para realizar o aporte.");
        }
    }


    // ====================================================
    // VALIDAÇÃO DO RESGATE
    // ====================================================

    /// <summary>
    /// Rejeita resgates cujo valor excede o valor atual
    /// da aplicação financeira.
    /// </summary>
  /// <summary>
/// Valida um resgate considerando o valor disponível
/// na data da movimentação, e não apenas o valor atual.
/// </summary>
private async Task ValidarResgateHistoricoAsync(
    AplicacaoFinanceira aplicacao,
    int usuarioId,
    DateTime dataMovimentacao,
    decimal valor)
{
    var dataReferencia =
        DateOnly.FromDateTime(dataMovimentacao);

    // Busca o último fechamento anterior à data.
    var historicoAnterior =
        await _historicoValorAplicacaoRepository
            .ObterUltimoAntesDaDataAsync(
                aplicacao.Id,
                dataReferencia);

    // Consulta as movimentações da aplicação.
    var movimentacoes =
        await _movimentacaoRepository
            .ListarAteDataAsync(
                aplicacao.Id,
                usuarioId,
                dataMovimentacao);

    // Como o histórico anterior é um fechamento diário,
    // só consideramos operações realizadas depois dele.
    var movimentacoesPosterioresAoFechamento =
        movimentacoes.Where(m =>
            historicoAnterior is null ||
            DateOnly.FromDateTime(m.Data) >
                historicoAnterior.DataReferencia);

    var impactoMovimentacoes =
        movimentacoesPosterioresAoFechamento.Sum(m =>
            m.Tipo == TipoMovimentacaoAplicacao.Aporte
                ? m.Valor
                : -m.Valor);

    var valorDisponivel =
        (historicoAnterior?.Valor ?? 0m)
        + impactoMovimentacoes;

    if (valor > valorDisponivel)
    {
        throw new ValidacaoException(
            "O valor do resgate é maior que o saldo " +
            "disponível na data da movimentação.");
    }
}


    // ====================================================
    // MAPEAMENTO
    // ====================================================

    /// <summary>
    /// Converte uma movimentação para o DTO de resposta.
    /// </summary>
    private static
        MovimentacaoAplicacaoResponseDto Mapear(
            MovimentacaoAplicacao movimentacao)
    {
        return new MovimentacaoAplicacaoResponseDto
        {
            Id =
                movimentacao.Id,

            Valor =
                movimentacao.Valor,

            Data =
                movimentacao.Data,

            Tipo =
                (int)movimentacao.Tipo,

            TipoDescricao =
                movimentacao.Tipo.ToString(),

            AplicacaoFinanceiraId =
                movimentacao.AplicacaoFinanceiraId,

            Conta =
                movimentacao.Conta?.Nome
                ?? string.Empty,

            ContaId =
                movimentacao.ContaId
        };
    }

    private async Task ValidarSaldosPosterioresAsync(
        AplicacaoFinanceira aplicacao,
        DateTime dataMovimentacao,
        decimal valorResgate)
    {
    var dataReferencia =
        DateOnly.FromDateTime(dataMovimentacao);

    var historicosPosteriores =
        await _historicoValorAplicacaoRepository
            .ListarAPartirDaDataAsync(
                aplicacao.Id,
                dataReferencia);

    foreach (var historico in historicosPosteriores
        .OrderBy(h => h.DataReferencia))
    {
        // O fechamento de mercado é uma referência
        // absoluta e interrompe a propagação.
        if (historico.Origem ==
            OrigemHistoricoAplicacao.AtualizacaoMercado)
        {
            break;
        }

        // A nova movimentação reduziria este saldo.
        var saldoProjetado =
            historico.Valor - valorResgate;

        if (saldoProjetado < 0)
        {
            throw new ValidacaoException(
                "O resgate não pode ser realizado, " +
                "pois deixaria o saldo da aplicação " +
                "negativo em uma data posterior: " +
                historico.DataReferencia
                    .ToString("dd/MM/yyyy"));
        }
    }
  }

 private async Task ValidarSequenciaCronologicaAsync(
    AplicacaoFinanceira aplicacao,
    int usuarioId,
    DateTime dataNovoResgate,
    decimal valorNovoResgate)
{
    var dataReferencia =
        DateOnly.FromDateTime(dataNovoResgate);

    // O último snapshot do dia anterior ou mais antigo
    // é o saldo inicial da simulação.
    var historicoAnterior =
        await _historicoValorAplicacaoRepository
            .ObterUltimoAntesDaDataAsync(
                aplicacao.Id,
                dataReferencia);

    decimal saldo = historicoAnterior?.Valor ?? 0m;

    var movimentacoes =
        await _movimentacaoRepository
            .ListarCronologicamenteAsync(
                aplicacao.Id,
                usuarioId);

    var historicos =
        await _historicoValorAplicacaoRepository
            .ListarPorAplicacaoAsync(
                aplicacao.Id,
                usuarioId);

    // Fechamentos explícitos a partir da data
    // da nova movimentação.
    var fechamentosMercado = historicos
        .Where(h =>
            h.Origem ==
                OrigemHistoricoAplicacao.AtualizacaoMercado &&
            h.DataReferencia >= dataReferencia)
        .OrderBy(h => h.DataReferencia)
        .ToList();

    // Uma atualização de mercado posterior funciona
    // como barreira: a nova operação não altera
    // os saldos depois daquele fechamento.
    var primeiroFechamento =
        fechamentosMercado.FirstOrDefault();

    var operacoes = movimentacoes
        .Where(m =>
            DateOnly.FromDateTime(m.Data) >= dataReferencia &&
            (primeiroFechamento is null ||
             DateOnly.FromDateTime(m.Data) <=
                 primeiroFechamento.DataReferencia))
        .Select(m => new
        {
            m.Data,
            m.Tipo,
            m.Valor,
            m.Id,
            Nova = false
        })
        .ToList();

    // Simula o novo resgate sem gravá-lo.
    operacoes.Add(new
    {
        Data = dataNovoResgate,
        Tipo = TipoMovimentacaoAplicacao.Resgate,
        Valor = valorNovoResgate,
        Id = int.MaxValue,
        Nova = true
    });

    // Para horários iguais, a nova movimentação
    // será considerada depois das já registradas.
    var operacoesOrdenadas = operacoes
        .OrderBy(m => m.Data)
        .ThenBy(m => m.Id)
        .ToList();

    foreach (var operacao in operacoesOrdenadas)
    {
        if (operacao.Tipo ==
            TipoMovimentacaoAplicacao.Aporte)
        {
            saldo += operacao.Valor;
        }
        else
        {
            saldo -= operacao.Valor;
        }

        if (saldo < 0)
        {
            throw new ValidacaoException(
                "O resgate não pode ser realizado, " +
                "pois deixaria o saldo da aplicação " +
                "negativo em " +
                operacao.Data.ToString(
                    "dd/MM/yyyy HH:mm") + ".");
        }
    }

    // Não é necessário continuar além do primeiro
    // fechamento de mercado, pois ele redefine
    // o valor absoluto da aplicação.
}


}