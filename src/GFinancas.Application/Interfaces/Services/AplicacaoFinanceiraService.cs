using GFinancas.Application.DTOs.Aplicacoes;
using GFinancas.Application.Exceptions;
using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Application.Interfaces.Services;
using GFinancas.Domain.Entities;
using GFinancas.Domain.Enums;

namespace GFinancas.Application.Services;

/// <summary>
/// Valida e gerencia as aplicações financeiras do usuário autenticado e calcula seus indicadores.
/// </summary>
public class AplicacaoFinanceiraService
    : IAplicacaoFinanceiraService
{
    private readonly IAplicacaoFinanceiraRepository
        _aplicacaoRepository;

    private readonly IMovimentacaoAplicacaoRepository
        _movimentacaoRepository;

    private readonly IUsuarioAtualService
        _usuarioAtualService;

    private readonly IHistoricoValorAplicacaoRepository
    _historicoRepository;

    private readonly IUnitOfWork _unitOfWork;


    /// <summary>
    /// Recebe e armazena as dependências utilizadas pelo componente.
    /// </summary>
    /// <param name="aplicacaoRepository">Repositório de aplicações financeiras.</param>
    /// <param name="movimentacaoRepository">Repositório de movimentações e totais de aplicações.</param>
    /// <param name="usuarioAtualService">Serviço que identifica o usuário autenticado.</param>
    public AplicacaoFinanceiraService(
        IAplicacaoFinanceiraRepository aplicacaoRepository,
        IMovimentacaoAplicacaoRepository movimentacaoRepository,
        IUsuarioAtualService usuarioAtualService,
        IHistoricoValorAplicacaoRepository historicoRepository,
        IUnitOfWork unitOfWork)
    {
        _aplicacaoRepository = aplicacaoRepository;
        _movimentacaoRepository = movimentacaoRepository;
        _usuarioAtualService = usuarioAtualService;
        _historicoRepository = historicoRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Valida os dados e cadastra uma aplicação ativa com valor inicial zero para o usuário autenticado.
    /// </summary>
    /// <param name="dto">Dados recebidos para a operação.</param>
    public async Task<AplicacaoFinanceiraResponseDto>
        CriarAsync(
            CriarAplicacaoFinanceiraDto dto)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;

        ValidarNome(dto.Nome);
        ValidarInstituicao(dto.Instituicao);

        if (!Enum.IsDefined(typeof(TipoAplicacao), dto.Tipo))
        {
            throw new ValidacaoException(
                "O tipo da aplicação é inválido.");
        }

        if (dto.DataInicio == default)
        {
            throw new ValidacaoException(
                "A data inicial é obrigatória.");
        }

        var aplicacao =
             new AplicacaoFinanceira
             {
                Nome = dto.Nome.Trim(),
                Tipo = dto.Tipo,
                Instituicao = dto.Instituicao.Trim(),
                DataInicio = dto.DataInicio,
                ValorAtual = 0,
                Ativa = true,
                UsuarioId = usuarioId
            };

        await _aplicacaoRepository
            .AdicionarAsync(aplicacao);

        return await MontarResponseAsync(
            aplicacao);
    }

    /// <summary>
    /// Lista as aplicações financeiras do usuário.
    /// </summary>
    public async Task<
        IEnumerable<AplicacaoFinanceiraResponseDto>>
        ListarAsync()
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;

        var aplicacoes =
            await _aplicacaoRepository
                .ListarAsync(usuarioId);

        var resultado =
            new List<AplicacaoFinanceiraResponseDto>();

        foreach (var aplicacao in aplicacoes)
        {
            resultado.Add(
                await MontarResponseAsync(aplicacao)
            );
        }

        return resultado;
    }

    /// <summary>
    /// Busca a aplicação pelo identificador, restringindo a consulta ao usuário.
    /// </summary>
    /// <param name="id">Identificador da aplicação financeira.</param>
    public async Task<AplicacaoFinanceiraResponseDto>
        ObterPorIdAsync(int id)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;

        var aplicacao =
            await _aplicacaoRepository
                .ObterPorIdAsync(
                    id,
                    usuarioId);

        if (aplicacao is null)
        {
            throw new NaoEncontradoException(
                "Aplicação financeira não encontrada.");
        }

        return await MontarResponseAsync(
            aplicacao);
    }

    /// <summary>
    /// Valida e atualiza o nome e a instituição da aplicação do usuário autenticado.
    /// </summary>
    /// <param name="id">Identificador da aplicação financeira.</param>
    /// <param name="dto">Dados recebidos para a operação.</param>
    public async Task<AplicacaoFinanceiraResponseDto> AtualizarAsync(
        int id,
        AtualizarAplicacaoFinanceiraDto dto)
    {
        var usuarioId = _usuarioAtualService.UsuarioId;

        ValidarNome(dto.Nome);
        ValidarInstituicao(dto.Instituicao);

        var aplicacao = await _aplicacaoRepository
            .ObterPorIdAsync(id, usuarioId);

        if (aplicacao is null)
        {
            throw new NaoEncontradoException(
                "Aplicação financeira não encontrada.");
        }

        aplicacao.Nome = dto.Nome.Trim();
        aplicacao.Instituicao = dto.Instituicao.Trim();

        await _aplicacaoRepository.AtualizarAsync(aplicacao);

        return await MontarResponseAsync(aplicacao);
    }

    /// <summary>
    /// Atualiza o histórico de valores e altera o valor atual quando a data informada é a mais recente.
    /// </summary>
    /// <param name="id">Identificador da aplicação financeira.</param>
    /// <param name="dto">Valor e data de referência informados.</param>
    public async Task<AplicacaoFinanceiraResponseDto> AtualizarValorAsync(
    int id,
    AtualizarValorAplicacaoDto dto)
{
    var usuarioId =
        _usuarioAtualService.UsuarioId;


    // =====================================================
    // 1. VALIDAR VALOR
    // =====================================================

    if (dto.Valor < 0)
    {
        throw new ValidacaoException(
            "O valor da aplicação não pode ser negativo.");
    }


    // =====================================================
    // 2. BUSCAR APLICAÇÃO
    // =====================================================

    var aplicacao =
        await _aplicacaoRepository
            .ObterPorIdAsync(
                id,
                usuarioId);


    if (aplicacao is null)
    {
        throw new NaoEncontradoException(
            "Aplicação financeira não encontrada.");
    }


    // =====================================================
    // 3. VERIFICAR SE ESTÁ ATIVA
    // =====================================================

    if (!aplicacao.Ativa)
    {
        throw new ValidacaoException(
            "Não é possível atualizar o valor de uma aplicação inativa.");
    }


    // =====================================================
    // 4. VALIDAR DATA
    // =====================================================

    var hoje =
        DateOnly.FromDateTime(
            DateTime.Today);


    if (dto.DataReferencia > hoje)
    {
        throw new ValidacaoException(
            "A data de referência não pode estar no futuro.");
    }


    var dataInicio =
        DateOnly.FromDateTime(
            aplicacao.DataInicio);


    if (dto.DataReferencia < dataInicio)
    {
        throw new ValidacaoException(
            "A data de referência não pode ser anterior à data de início da aplicação.");
    }


    // =====================================================
    // 5. VERIFICAR SE JÁ EXISTE HISTÓRICO NESSA DATA
    // =====================================================

    var historicoExistente =
        await _historicoRepository
            .ObterPorAplicacaoEDataAsync(
                aplicacao.Id,
                dto.DataReferencia);


    // =====================================================
    // 6. DESCOBRIR O HISTÓRICO MAIS RECENTE
    // =====================================================

    var ultimoHistorico =
        await _historicoRepository
            .ObterMaisRecenteAsync(
                aplicacao.Id);


    // =====================================================
    // 7. DESCOBRIR SE VALORATUAL DEVE SER ALTERADO
    //
    // Uma atualização retroativa do histórico não deve
    // substituir o valor atual se existir um snapshot
    // posterior.
    // =====================================================

    var deveAtualizarValorAtual =
        ultimoHistorico is null ||
        dto.DataReferencia >=
        ultimoHistorico.DataReferencia;


    // =====================================================
    // 8. EXECUTAR AS ALTERAÇÕES NA MESMA TRANSAÇÃO
    // =====================================================

    await _unitOfWork.ExecutarEmTransacaoAsync(
        async () =>
        {
            // ---------------------------------------------
            // Atualiza ValorAtual somente quando a posição
            // informada for a mais recente.
            // ---------------------------------------------

            if (deveAtualizarValorAtual)
            {
                aplicacao.ValorAtual =
                    dto.Valor;


                await _aplicacaoRepository
                    .AtualizarAsync(
                        aplicacao);
            }


            // ---------------------------------------------
            // Ainda não existe histórico nessa data.
            //
            // Como o valor foi informado explicitamente
            // pelo usuário, sua origem é uma atualização
            // de mercado.
            // ---------------------------------------------

            if (historicoExistente is null)
            {
                var novoHistorico =
                    new HistoricoValorAplicacao
                    {
                        AplicacaoFinanceiraId =
                            aplicacao.Id,

                        DataReferencia =
                            dto.DataReferencia,

                        Valor =
                            dto.Valor,

                        Origem =
                            OrigemHistoricoAplicacao
                                .AtualizacaoMercado
                    };


                await _historicoRepository
                    .AdicionarAsync(
                        novoHistorico);
            }

            // ---------------------------------------------
            // Já existe histórico nessa data.
            //
            // O PATCH AtualizarValor representa uma
            // informação explícita do usuário.
            //
            // Portanto:
            //
            // 1. substituímos o valor;
            // 2. transformamos sua origem em
            //    AtualizacaoMercado.
            // ---------------------------------------------

            else
            {
                historicoExistente.Valor =
                    dto.Valor;


                historicoExistente.Origem =
                    OrigemHistoricoAplicacao
                        .AtualizacaoMercado;


                await _historicoRepository
                    .AtualizarAsync(
                        historicoExistente);
            }
        });


    // =====================================================
    // 9. DEVOLVER A APLICAÇÃO
    // =====================================================

    return await MontarResponseAsync(
        aplicacao);
}

    /// <summary>
    /// Desativa uma aplicação do usuário autenticado, rejeitando registros inexistentes ou já desativados.
    /// </summary>
    /// <param name="id">Identificador da aplicação financeira.</param>
    public async Task DesativarAsync(
        int id)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;

        var aplicacao =
            await _aplicacaoRepository
                .ObterPorIdAsync(
                    id,
                    usuarioId);

        if (aplicacao is null)
        {
            throw new NaoEncontradoException(
                "Aplicação financeira não encontrada.");
        }

        if (!aplicacao.Ativa)
        {
            throw new ValidacaoException(
                "A aplicação já está desativada.");
        }

        aplicacao.Ativa = false;

        await _aplicacaoRepository
            .AtualizarAsync(aplicacao);
    }

    /// <summary>
    /// Calcula capital líquido, resultado e rentabilidade e monta a resposta da aplicação; usa rentabilidade zero quando o capital líquido não é positivo.
    /// </summary>
    /// <param name="aplicacao">Aplicação financeira utilizada na operação.</param>
    private async Task<AplicacaoFinanceiraResponseDto> MontarResponseAsync(
    AplicacaoFinanceira aplicacao)
{
    var totalAportado =
        await _movimentacaoRepository
            .ObterTotalAportesAsync(aplicacao.Id);

    var totalResgatado =
        await _movimentacaoRepository
            .ObterTotalResgatesAsync(aplicacao.Id);

    var capitalLiquido =
        totalAportado - totalResgatado;

    var resultado =
        aplicacao.ValorAtual
        + totalResgatado
        - totalAportado;

    var rentabilidadePercentual =
        totalAportado > 0
            ? resultado / totalAportado * 100
            : 0;

    return new AplicacaoFinanceiraResponseDto
    {
        Id = aplicacao.Id,

        Nome = aplicacao.Nome,

        Tipo = (int)aplicacao.Tipo,

        TipoDescricao =
            aplicacao.Tipo.ToString(),

        Instituicao =
            aplicacao.Instituicao,

        DataInicio =
            aplicacao.DataInicio,

        TotalAportado =
            totalAportado,

        TotalResgatado =
            totalResgatado,

        CapitalLiquido =
            capitalLiquido,

        ValorAtual =
            aplicacao.ValorAtual,

        Resultado =
            resultado,

        RentabilidadePercentual =
            rentabilidadePercentual,

        Ativa =
            aplicacao.Ativa
    };
}

    /// <summary>
    /// Exige nome preenchido com no máximo 150 caracteres.
    /// </summary>
    /// <param name="nome">Nome da aplicação financeira.</param>
    private static void ValidarNome(
        string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ValidacaoException(
                "O nome da aplicação é obrigatório.");
        }

        if (nome.Length > 150)
        {
            throw new ValidacaoException(
                "O nome da aplicação deve possuir no máximo 150 caracteres.");
        }
    }

    /// <summary>
    /// Exige instituição preenchida com no máximo 150 caracteres.
    /// </summary>
    /// <param name="instituicao">Nome da instituição financeira.</param>
    private static void ValidarInstituicao(
        string instituicao)
    {
        if (string.IsNullOrWhiteSpace(
            instituicao))
        {
            throw new ValidacaoException(
                "A instituição é obrigatória.");
        }

        if (instituicao.Length > 150)
        {
            throw new ValidacaoException(
                "A instituição deve possuir no máximo 150 caracteres.");
        }
    }

   public async Task<IEnumerable<HistoricoValorAplicacaoResponseDto>>
        ListarHistoricoValoresAsync(
            int aplicacaoId)
    {
        var usuarioId =
        _usuarioAtualService.UsuarioId;


    // Verifica se a aplicação pertence
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


    var historicos =
        await _historicoRepository
            .ListarPorAplicacaoAsync(
                aplicacaoId,
                usuarioId);


    return historicos
        .Select(h =>
            new HistoricoValorAplicacaoResponseDto
            {
                Id =
                    h.Id,

                DataReferencia =
                    h.DataReferencia,

                Valor =
                    h.Valor,

                Origem =
                    (int)h.Origem,

                OrigemDescricao =
                    h.Origem.ToString()
            })
        .ToList();
    }

}