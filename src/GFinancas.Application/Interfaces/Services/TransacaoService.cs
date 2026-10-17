using GFinancas.Application.Dto.Transacoes;
using GFinancas.Application.DTOs.Comum;
using GFinancas.Application.DTOs.Transacoes;
using GFinancas.Application.Exceptions;
using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Application.Interfaces.Repository;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Application.Interfaces.Services;
using GFinancas.Domain.Entities;
using GFinancas.Domain.Enums;

namespace GFinancas.Application.Services;

/// <summary>
/// Aplica as regras de negócio de transações e prepara os dados de resposta.
/// </summary>
public class TransacaoService : ITransacaoService
{
/// <summary>
/// Repositório usado para acessar os dados de transações.
/// </summary>
private readonly ITransacaoRepository _transacaoRepository;
/// <summary>
/// Repositório usado para acessar os dados de contas.
/// </summary>
private readonly IContaRepository _contaRepository;
/// <summary>
/// Repositório usado para acessar os dados de categorias.
/// </summary>
private readonly ICategoriaRepository _categoriaRepository;
/// <summary>
/// Repositório usado para acessar os dados de usuários.
/// </summary>
private readonly IUsuarioRepository _usuarioRepository;
/// <summary>
/// Serviço que fornece o identificador do usuário da requisição atual.
/// </summary>
private readonly IUsuarioAtualService _usuarioAtualService;

/// <summary>
/// Recebe e armazena as dependências necessárias às operações desta classe.
/// </summary>
/// <param name="transacaoRepository">Repositório usado para acessar os dados de transações.</param>
/// <param name="contaRepository">Repositório usado para acessar os dados de contas.</param>
/// <param name="categoriaRepository">Repositório usado para acessar os dados de categorias.</param>
/// <param name="usuarioRepository">Repositório usado para acessar os dados de usuários.</param>
/// <param name="usuarioAtualService">Serviço que fornece o identificador do usuário da requisição atual.</param>
public TransacaoService(
    ITransacaoRepository transacaoRepository,
    IContaRepository contaRepository,
    ICategoriaRepository categoriaRepository,
    IUsuarioRepository usuarioRepository,
    IUsuarioAtualService usuarioAtualService)
{
    _transacaoRepository = transacaoRepository;
    _contaRepository = contaRepository;
    _categoriaRepository = categoriaRepository;
    _usuarioRepository = usuarioRepository;
    _usuarioAtualService = usuarioAtualService;
}

    /// <summary>
    /// Valida os dados e a propriedade da conta e da categoria antes de cadastrar a transação.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    public async Task<TransacaoResponseDto> CriarAsync(CriarTransacaoDto dto)
{
    // Identificador do usuário autenticado, utilizado para associar ou restringir os dados.
    var usuarioId = _usuarioAtualService.UsuarioId;

    if (string.IsNullOrWhiteSpace(dto.Descricao))
        throw new ValidacaoException("A descrição é obrigatória.");

    if (dto.Valor <= 0)
        throw new ValidacaoException("O valor deve ser maior que zero.");

    if (!Enum.IsDefined(typeof(TipoTransacao), dto.Tipo))
        throw new ValidacaoException("Tipo de transação inválido.");

    // Registro de conta consultado para esta operação; sua existência é verificada antes do uso.
    var conta = await _contaRepository.ObterPorIdAsync(dto.ContaId);

    if (conta is null || conta.UsuarioId != usuarioId)
        throw new NaoEncontradoException("Conta não encontrada.");

    // Registro de categoria consultado para esta operação; sua existência é verificada antes do uso.
    var categoria = await _categoriaRepository
        .ObterPorIdAsync(dto.CategoriaId);

    if (categoria is null || categoria.UsuarioId != usuarioId)
        throw new NaoEncontradoException("Categoria não encontrada.");

    if (categoria.Tipo != dto.Tipo)
    {
        throw new ValidacaoException(
            "O tipo da categoria deve ser igual ao tipo da transação.");
    }

    // Dados de transação preparados para persistência após as validações.
    var transacao = new Transacao
    {
        Descricao = dto.Descricao.Trim(),
        Valor = dto.Valor,
        Data = dto.Data,
        Tipo = dto.Tipo,
        ContaId = conta.Id,
        CategoriaId = categoria.Id
    };

    await _transacaoRepository.AdicionarAsync(transacao);

    return new TransacaoResponseDto
    {
        Id = transacao.Id,
        Descricao = transacao.Descricao,
        Valor = transacao.Valor,
        Data = transacao.Data,
        Tipo = (int)transacao.Tipo,
        TipoDescricao = transacao.Tipo.ToString(),

        ContaId = conta.Id,
        Conta = conta.Nome,

        CategoriaId = categoria.Id,
        Categoria = categoria.Nome
    };
}

    /// <summary>
    /// Lista transações cadastrados no sistema e converte os registros em DTOs de resposta.
    /// </summary>
    public async Task<IEnumerable<TransacaoResponseDto>> ListarAsync()
    {
        // Transações obtidas na consulta para compor a resposta ou calcular os indicadores.
        var transacoes =
            await _transacaoRepository.ObterTodasAsync();

        return transacoes.Select(MapearParaResponse);
    }

   /// <summary>
   /// Busca transação pelo identificador e verifica se pertence ao usuário atual.
   /// </summary>
   /// <param name="id">Identificador do registro procurado ou alterado.</param>
   public async Task<TransacaoResponseDto?> ObterPorIdAsync(int id)
{
    // Identificador do usuário autenticado, utilizado para associar ou restringir os dados.
    var usuarioId = _usuarioAtualService.UsuarioId;

    // Registro de transação consultado para esta operação; sua existência é verificada antes do uso.
    var transacao = await _transacaoRepository.ObterPorIdAsync(id);

    if (transacao is null)
        throw new NaoEncontradoException("Transação não encontrada.");

    if (transacao.Conta is null ||
        transacao.Conta.UsuarioId != usuarioId)
    {
        throw new NaoEncontradoException("Transação não encontrada.");
    }

    return MapearParaResponse(transacao);
}

    /// <summary>
    /// Converte os dados de transação para o DTO de resposta.
    /// </summary>
    /// <param name="transacao">Transação utilizada na operação.</param>
    /// <param name="conta">Conta financeira utilizada na operação.</param>
    /// <param name="categoria">Categoria utilizada na operação.</param>
    private static TransacaoResponseDto MapearParaResponse(
    Transacao transacao,
    Conta conta,
    Categoria categoria)
{
    return new TransacaoResponseDto
    {
        Id = transacao.Id,
        Descricao = transacao.Descricao,
        Valor = transacao.Valor,
        Data = transacao.Data,

        Tipo = (int)transacao.Tipo,
        TipoDescricao = transacao.Tipo.ToString(),

        ContaId = conta.Id,
        Conta = conta.Nome,

        CategoriaId = categoria.Id,
        Categoria = categoria.Nome
    };
}
    /// <summary>
    /// Converte os dados de transação para o DTO de resposta.
    /// </summary>
    /// <param name="transacao">Transação utilizada na operação.</param>
    private static TransacaoResponseDto MapearParaResponse(
        Transacao transacao)
    {
        return new TransacaoResponseDto
        {
            Id = transacao.Id,
            Descricao = transacao.Descricao,
            Valor = transacao.Valor,
            Data = transacao.Data,

            Tipo = (int)transacao.Tipo,
            TipoDescricao = transacao.Tipo.ToString(),

            ContaId = transacao.ContaId,
            Conta = transacao.Conta?.Nome ?? string.Empty,

            CategoriaId = transacao.CategoriaId,
            Categoria = transacao.Categoria?.Nome ?? string.Empty
        };
    }

    /// <summary>
    /// Valida o usuário e os filtros informados e retorna as transações correspondentes.
    /// </summary>
    /// <param name="filtro">Usuário e critérios de seleção das transações.</param>
    public async Task<IEnumerable<TransacaoResponseDto>> FiltrarAsync(
    FiltroTransacaoDto filtro)
{
    if (filtro.UsuarioId <= 0)
    {
        throw new ValidacaoException(
            "O usuário é obrigatório.");
    }

    if (filtro.DataInicio.HasValue &&
        filtro.DataFim.HasValue &&
        filtro.DataInicio.Value > filtro.DataFim.Value)
    {
        throw new ValidacaoException(
            "A data inicial não pode ser maior que a data final.");
    }

    if (filtro.Tipo.HasValue &&
        !Enum.IsDefined(
            typeof(TipoTransacao),
            filtro.Tipo.Value))
    {
        throw new ValidacaoException(
            "Tipo de transação inválido.");
    }

    // Registro de usuário consultado para esta operação; sua existência é verificada antes do uso.
    var usuario =
        await _usuarioRepository.ObterPorIdAsync(
            filtro.UsuarioId);

    if (usuario is null)
    {
        throw new NaoEncontradoException(
            "Usuário não encontrado.");
    }

    // Transações obtidas na consulta para compor a resposta ou calcular os indicadores.
    var transacoes =
        await _transacaoRepository.FiltrarAsync(
            filtro.UsuarioId,
            filtro.DataInicio,
            filtro.DataFim,
            filtro.Tipo,
            filtro.ContaId,
            filtro.CategoriaId);

    return transacoes.Select(MapearParaResponse);
}

   /// <summary>
   /// Valida a transação e os vínculos do usuário atual antes de salvar os novos dados.
   /// </summary>
   /// <param name="id">Identificador do registro procurado ou alterado.</param>
   /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
   public async Task<TransacaoResponseDto> AtualizarAsync(
    int id,
    AtualizarTransacaoDto dto)
{
    // Identificador do usuário autenticado, utilizado para associar ou restringir os dados.
    var usuarioId = _usuarioAtualService.UsuarioId;

    // Registro de transação consultado para esta operação; sua existência é verificada antes do uso.
    var transacao = await _transacaoRepository.ObterPorIdAsync(id);

    if (transacao is null ||
        transacao.Conta is null ||
        transacao.Conta.UsuarioId != usuarioId)
    {
        throw new NaoEncontradoException("Transação não encontrada.");
    }

    if (string.IsNullOrWhiteSpace(dto.Descricao))
        throw new ValidacaoException("A descrição é obrigatória.");

    if (dto.Valor <= 0)
        throw new ValidacaoException("O valor deve ser maior que zero.");

    if (!Enum.IsDefined(typeof(TipoTransacao), dto.Tipo))
        throw new ValidacaoException("Tipo de transação inválido.");

    // Registro de conta consultado para esta operação; sua existência é verificada antes do uso.
    var conta = await _contaRepository.ObterPorIdAsync(dto.ContaId);

    if (conta is null || conta.UsuarioId != usuarioId)
        throw new NaoEncontradoException("Conta não encontrada.");

    // Registro de categoria consultado para esta operação; sua existência é verificada antes do uso.
    var categoria = await _categoriaRepository
        .ObterPorIdAsync(dto.CategoriaId);

    if (categoria is null || categoria.UsuarioId != usuarioId)
        throw new NaoEncontradoException("Categoria não encontrada.");

    if (categoria.Tipo != dto.Tipo)
    {
        throw new ValidacaoException(
            "O tipo da categoria deve ser igual ao tipo da transação.");
    }

    transacao.Descricao = dto.Descricao.Trim();
    transacao.Valor = dto.Valor;
    transacao.Data = dto.Data;
    transacao.Tipo = dto.Tipo;
    transacao.ContaId = conta.Id;
    transacao.CategoriaId = categoria.Id;

    await _transacaoRepository.AtualizarAsync(transacao);

    return new TransacaoResponseDto
    {
        Id = transacao.Id,
        Descricao = transacao.Descricao,
        Valor = transacao.Valor,
        Data = transacao.Data,
        Tipo = (int)transacao.Tipo,
        TipoDescricao = transacao.Tipo.ToString(),

        ContaId = conta.Id,
        Conta = conta.Nome,

        CategoriaId = categoria.Id,
        Categoria = categoria.Nome
    };
}


/// <summary>
/// Verifica se a transação pertence ao usuário atual antes de removê-la.
/// </summary>
/// <param name="id">Identificador do registro procurado ou alterado.</param>
public async Task RemoverAsync(int id)
{
    // Identificador do usuário autenticado, utilizado para associar ou restringir os dados.
    var usuarioId = _usuarioAtualService.UsuarioId;

    // Registro de transação consultado para esta operação; sua existência é verificada antes do uso.
    var transacao = await _transacaoRepository.ObterPorIdAsync(id);

    if (transacao is null ||
        transacao.Conta is null ||
        transacao.Conta.UsuarioId != usuarioId)
    {
        throw new NaoEncontradoException("Transação não encontrada.");
    }

    await _transacaoRepository.RemoverAsync(transacao);
}

    /// <summary>
    /// Valida descrição, valor, tipo e compatibilidade entre conta, categoria e seus proprietários.
    /// </summary>
    /// <param name="descricao">Descrição da movimentação a validar.</param>
    /// <param name="valor">Valor da movimentação, que deve ser positivo.</param>
    /// <param name="tipo">Tipo de movimentação: receita ou despesa.</param>
    /// <param name="contaId">Identificador da conta financeira.</param>
    /// <param name="categoriaId">Identificador da categoria.</param>
    private async Task<(Conta conta, Categoria categoria)>
    ValidarTransacaoAsync(
        string descricao,
        decimal valor,
        TipoTransacao tipo,
        int contaId,
        int categoriaId)
{
    if (string.IsNullOrWhiteSpace(descricao))
    {
        throw new ValidacaoException(
            "A descrição da transação é obrigatória.");
    }

    if (valor <= 0)
    {
        throw new ValidacaoException(
            "O valor da transação deve ser maior que zero.");
    }

    if (!Enum.IsDefined(typeof(TipoTransacao), tipo))
    {
        throw new ValidacaoException(
            "Tipo de transação inválido.");
    }

    // Registro de conta consultado para esta operação; sua existência é verificada antes do uso.
    var conta =
        await _contaRepository.ObterPorIdAsync(contaId);

    if (conta is null)
    {
        throw new NaoEncontradoException(
            "Conta não encontrada.");
    }

    // Registro de categoria consultado para esta operação; sua existência é verificada antes do uso.
    var categoria =
        await _categoriaRepository.ObterPorIdAsync(categoriaId);

    if (categoria is null)
    {
        throw new NaoEncontradoException(
            "Categoria não encontrada.");
    }

    if (conta.UsuarioId != categoria.UsuarioId)
    {
        throw new ValidacaoException(
            "A conta e a categoria devem pertencer ao mesmo usuário.");
    }

    if (categoria.Tipo != tipo)
    {
        throw new ValidacaoException(
            "O tipo da categoria não corresponde ao tipo da transação.");
    }

    return (conta, categoria);
}

 /// <summary>
 /// Consulta as transações do usuário atual e monta os itens e metadados de paginação.
 /// </summary>
 /// <param name="pagina">Número da página solicitada, começando em 1.</param>
 /// <param name="tamanhoPagina">Quantidade de itens solicitada por página.</param>
 /// <remarks>
 /// Nas expressões lambda, <c>t</c> representa a transação na expressão de consulta, cálculo ou projeção.
 /// </remarks>
 public async Task<ResultadoPaginadoDto<TransacaoResponseDto>>
    ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina)
{
    // Identificador do usuário autenticado, utilizado para associar ou restringir os dados.
    var usuarioId = _usuarioAtualService.UsuarioId;

    // Itens e total de transações retornados pela consulta paginada do repositório.
    var resultado = await _transacaoRepository
        .ObterPaginadoAsync(
            usuarioId,
            pagina,
            tamanhoPagina);

    // Número de páginas, arredondando para cima a divisão do total pelo tamanho da página.
    var totalPaginas = (int)Math.Ceiling(
        resultado.TotalItens / (double)tamanhoPagina);

    return new ResultadoPaginadoDto<TransacaoResponseDto>
    {
        Itens = resultado.Itens.Select(t => new TransacaoResponseDto
        {
            Id = t.Id,
            Descricao = t.Descricao,
            Valor = t.Valor,
            Data = t.Data,

            Tipo = (int)t.Tipo,
            TipoDescricao = t.Tipo.ToString(),

            ContaId = t.ContaId,
            Conta = t.Conta?.Nome ?? string.Empty,

            CategoriaId = t.CategoriaId,
            Categoria = t.Categoria?.Nome ?? string.Empty

        }).ToList(),

        PaginaAtual = pagina,
        TamanhoPagina = tamanhoPagina,
        TotalItens = resultado.TotalItens,
        TotalPaginas = totalPaginas
    };
}

}

