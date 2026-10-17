using GFinancas.Application.DTOs.Contas;
using GFinancas.Application.Exceptions;
using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Application.Interfaces.Repository;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Application.Interfaces.Services;
using GFinancas.Domain.Entities;
using GFinancas.Domain.Enums;

namespace GFinancas.Application.Services;

/// <summary>
/// Aplica as regras de negócio de contas e prepara os dados de resposta.
/// </summary>
public class ContaService : IContaService
{
    /// <summary>
    /// Repositório usado para acessar os dados de contas.
    /// </summary>
    private readonly IContaRepository _contaRepository;

    /// <summary>
    /// Repositório usado para acessar os dados de usuários.
    /// </summary>
    private readonly IUsuarioRepository _usuarioRepository;

    /// <summary>
    /// Repositório usado para acessar os dados de tipos de conta.
    /// </summary>
    private readonly ITipoContaRepository _tipoContaRepository;

    /// <summary>
    /// Repositório usado para acessar os dados de transações.
    /// </summary>
    private readonly ITransacaoRepository _transacaoRepository;

    /// <summary>
    /// Serviço que fornece o identificador do usuário da requisição atual.
    /// </summary>
    private readonly IUsuarioAtualService _usuarioAtualService;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    
    /// <summary>
    /// Serviço que calcula o saldo de uma conta.
    /// </summary>
    private readonly ISaldoContaService _saldoContaService;

    /// <summary>
    /// Repositório usado para acessar os dados de movimentações de aplicações financeiras.
    /// </summary>
    private readonly IMovimentacaoAplicacaoRepository
    _movimentacaoAplicacaoRepository;
    
    
    public ContaService(
        IContaRepository contaRepository,
        IUsuarioRepository usuarioRepository,
        ITipoContaRepository tipoContaRepository,
        ITransacaoRepository transacaoRepository,
        IUsuarioAtualService usuarioAtualService,
        ISaldoContaService saldoContaService,
        IMovimentacaoAplicacaoRepository movimentacaoAplicacaoRepository)
    {
        _contaRepository = contaRepository;
        _usuarioRepository = usuarioRepository;
        _tipoContaRepository = tipoContaRepository;
        _transacaoRepository = transacaoRepository;
        _usuarioAtualService = usuarioAtualService;
        _saldoContaService = saldoContaService;
        _movimentacaoAplicacaoRepository = movimentacaoAplicacaoRepository;
    }

    /// <summary>
    /// Valida nome e tipo de conta e cadastra a conta para o usuário atual.
    /// </summary>
    public async Task<ContaResponseDto> CriarAsync(
        CriarContaDto dto)
    {
        // ------------------------------------------------
        // 1. Identifica o usuário autenticado
        // ------------------------------------------------

        var usuarioId =
            _usuarioAtualService.UsuarioId;

        // ------------------------------------------------
        // 2. Valida o nome
        // ------------------------------------------------

        if (string.IsNullOrWhiteSpace(dto.Nome))
        {
            throw new ValidacaoException(
                "O nome da conta é obrigatório.");
        }

        // ------------------------------------------------
        // 3. Busca o tipo da conta
        // ------------------------------------------------

        var tipoConta =
            await _tipoContaRepository
                .ObterPorIdAsync(dto.TipoContaId);

        if (tipoConta is null)
        {
            throw new NaoEncontradoException(
                "Tipo de conta não encontrado.");
        }

        // ------------------------------------------------
        // 4. Monta a entidade
        // ------------------------------------------------

        var conta = new Conta
        {
            Nome = dto.Nome.Trim(),

            SaldoInicial = dto.SaldoInicial,

            // A data é definida somente na criação.
            DataCriacao =
                DateOnly.FromDateTime(DateTime.Today),

            // O proprietário da conta vem do JWT.
            UsuarioId = usuarioId,

            TipoContaId = tipoConta.Id
        };

        // ------------------------------------------------
        // 5. Persiste a conta
        // ------------------------------------------------

        await _contaRepository
            .AdicionarAsync(conta);

        /*
         * Como acabamos de criar a conta, atribuímos o
         * TipoConta carregado anteriormente para que o
         * método de mapeamento consiga retornar seu nome
         * sem precisar fazer outra consulta.
         */
        conta.TipoConta = tipoConta;

        // ------------------------------------------------
        // 6. Retorna o DTO
        // ------------------------------------------------

        return MapearParaResponse(conta);
    }

    /// <summary>
    /// Lista as contas pertencentes ao usuário autenticado.
    /// </summary>
    public async Task<IEnumerable<ContaResponseDto>>
        ListarAsync()
    {
        // ------------------------------------------------
        // 1. Identifica o usuário autenticado
        // ------------------------------------------------

        var usuarioId =
            _usuarioAtualService.UsuarioId;

        // ------------------------------------------------
        // 2. Busca somente as contas desse usuário
        // ------------------------------------------------

        var contas =
            await _contaRepository
                .ObterPorUsuarioIdAsync(usuarioId);

        // ------------------------------------------------
        // 3. Converte as entidades para DTO
        // ------------------------------------------------

        return contas
            .Select(MapearParaResponse)
            .ToList();
    }

    /// <summary>
    /// Busca uma conta pelo identificador e verifica se
    /// pertence ao usuário autenticado.
    /// </summary>
    public async Task<ContaResponseDto?>
        ObterPorIdAsync(int id)
    {
        // ------------------------------------------------
        // 1. Identifica o usuário autenticado
        // ------------------------------------------------

        var usuarioId =
            _usuarioAtualService.UsuarioId;

        // ------------------------------------------------
        // 2. Busca a conta
        // ------------------------------------------------

        var conta =
            await _contaRepository
                .ObterPorIdAsync(id);

        // ------------------------------------------------
        // 3. Verifica propriedade
        // ------------------------------------------------

        if (conta is null ||
            conta.UsuarioId != usuarioId)
        {
            throw new NaoEncontradoException(
                "Conta não encontrada.");
        }

        // ------------------------------------------------
        // 4. Retorna o DTO
        // ------------------------------------------------

        return MapearParaResponse(conta);
    }

    /// <summary>
    /// Converte uma entidade Conta para o DTO de resposta.
    ///
    /// Toda conversão de Conta para ContaResponseDto deve
    /// passar por este método para evitar propriedades
    /// esquecidas em diferentes endpoints.
    /// </summary>
    private static ContaResponseDto MapearParaResponse(
        Conta conta)
    {
        return new ContaResponseDto
        {
            Id = conta.Id,

            Nome = conta.Nome,

            SaldoInicial = conta.SaldoInicial,

            UsuarioId = conta.UsuarioId,

            Usuario =
                conta.Usuario?.Nome
                ?? string.Empty,

            TipoContaId = conta.TipoContaId,

            TipoConta =
                conta.TipoConta?.Nome
                ?? string.Empty,

            DataCriacao = conta.DataCriacao
        };
    }

    /// <summary>
    /// Verifica a propriedade da conta e calcula
    /// saldo inicial mais receitas menos despesas.
    /// </summary>
    public async Task<SaldoContaResponseDto>
    ObterSaldoAsync(int contaId)
{
    // ------------------------------------------------
    // 1. Identifica o usuário autenticado
    // ------------------------------------------------

    var usuarioId =
        _usuarioAtualService.UsuarioId;


    // ------------------------------------------------
    // 2. Busca a conta
    // ------------------------------------------------

    var conta =
        await _contaRepository
            .ObterPorIdAsync(contaId);


    // ------------------------------------------------
    // 3. Verifica se a conta pertence ao usuário
    // ------------------------------------------------

    if (conta is null ||
        conta.UsuarioId != usuarioId)
    {
        throw new NaoEncontradoException(
            "Conta não encontrada.");
    }


    // ------------------------------------------------
    // 4. Busca as transações para montar o detalhamento
    // ------------------------------------------------

    var transacoes =
        await _transacaoRepository
            .ObterPorContaIdAsync(contaId);


    // ------------------------------------------------
    // 5. Calcula receitas
    // ------------------------------------------------

    var totalReceitas =
        transacoes
            .Where(t =>
                t.Tipo == TipoTransacao.Receita)
            .Sum(t => t.Valor);


    // ------------------------------------------------
    // 6. Calcula despesas
    // ------------------------------------------------

    var totalDespesas =
        transacoes
            .Where(t =>
                t.Tipo == TipoTransacao.Despesa)
            .Sum(t => t.Valor);


    // ------------------------------------------------
    // 7. Obtém o saldo pela regra centralizada
    // ------------------------------------------------
    //
    // ISaldoContaService considera:
    //
    // SaldoInicial
    // + Receitas
    // - Despesas
    // - Aportes
    // + Resgates
    //
    // ------------------------------------------------

    var saldoAtual =
        await _saldoContaService
            .CalcularAsync(contaId);


    // ------------------------------------------------
    // 8. Retorna o resultado
    // ------------------------------------------------

    return new SaldoContaResponseDto
    {
        ContaId = conta.Id,

        Conta = conta.Nome,

        SaldoInicial =
            conta.SaldoInicial,

        TotalReceitas =
            totalReceitas,

        TotalDespesas =
            totalDespesas,

        SaldoAtual =
            saldoAtual
    };
}

    /// <summary>
    /// Valida os dados e atualiza uma conta pertencente
    /// ao usuário autenticado.
    /// </summary>
    public async Task<ContaResponseDto>
        AtualizarAsync(
            int id,
            AtualizarContaDto dto)
    {
        // ------------------------------------------------
        // 1. Identifica o usuário autenticado
        // ------------------------------------------------

        var usuarioId =
            _usuarioAtualService.UsuarioId;

        // ------------------------------------------------
        // 2. Valida nome
        // ------------------------------------------------

        if (string.IsNullOrWhiteSpace(dto.Nome))
        {
            throw new ValidacaoException(
                "O nome da conta é obrigatório.");
        }

        // ------------------------------------------------
        // 3. Busca a conta
        // ------------------------------------------------

        var conta =
            await _contaRepository
                .ObterPorIdAsync(id);

        // ------------------------------------------------
        // 4. Verifica propriedade
        // ------------------------------------------------

        if (conta is null ||
            conta.UsuarioId != usuarioId)
        {
            throw new NaoEncontradoException(
                "Conta não encontrada.");
        }

        // ------------------------------------------------
        // 5. Busca o novo tipo da conta
        // ------------------------------------------------

        var tipoConta =
            await _tipoContaRepository
                .ObterPorIdAsync(dto.TipoContaId);

        if (tipoConta is null)
        {
            throw new NaoEncontradoException(
                "Tipo de conta não encontrado.");
        }

        // ------------------------------------------------
        // 6. Atualiza somente propriedades editáveis
        // ------------------------------------------------

        conta.Nome =
            dto.Nome.Trim();

        conta.SaldoInicial =
            dto.SaldoInicial;

        conta.TipoContaId =
            tipoConta.Id;

        /*
         * DataCriacao NÃO deve ser alterada.
         *
         * Ela representa o momento em que a conta passou
         * a fazer parte do patrimônio.
         */

        // ------------------------------------------------
        // 7. Persiste
        // ------------------------------------------------

        await _contaRepository
            .AtualizarAsync(conta);

        /*
         * Garante que o mapeamento tenha acesso ao nome
         * do tipo selecionado.
         */
        conta.TipoConta = tipoConta;

        // ------------------------------------------------
        // 8. Retorna
        // ------------------------------------------------

        return MapearParaResponse(conta);
    }

    /// <summary>
    /// Remove uma conta pertencente ao usuário atual
    /// quando não possui transações.
    /// </summary>
   public async Task RemoverAsync(int id)
{
    // ------------------------------------------------
    // 1. Identifica o usuário autenticado
    // ------------------------------------------------

    var usuarioId =
        _usuarioAtualService.UsuarioId;


    // ------------------------------------------------
    // 2. Busca a conta
    // ------------------------------------------------

    var conta =
        await _contaRepository
            .ObterPorIdAsync(id);


    // ------------------------------------------------
    // 3. Verifica propriedade
    // ------------------------------------------------

    if (conta is null ||
        conta.UsuarioId != usuarioId)
    {
        throw new NaoEncontradoException(
            "Conta não encontrada.");
    }


    // ------------------------------------------------
    // 4. Verifica transações
    // ------------------------------------------------

    var transacoes =
        await _transacaoRepository
            .ObterPorContaIdAsync(id);

    if (transacoes.Any())
    {
        throw new ValidacaoException(
            "Não é possível excluir uma conta que possui transações.");
    }


    // ------------------------------------------------
    // 5. Verifica movimentações de investimentos
    // ------------------------------------------------

    var possuiMovimentacoesAplicacao =
        await _movimentacaoAplicacaoRepository
            .ExistePorContaAsync(id);

    if (possuiMovimentacoesAplicacao)
    {
        throw new ValidacaoException(
            "Não é possível excluir uma conta que possui movimentações de investimentos.");
    }


    // ------------------------------------------------
    // 6. Remove
    // ------------------------------------------------

    await _contaRepository
        .RemoverAsync(conta);
    }
}