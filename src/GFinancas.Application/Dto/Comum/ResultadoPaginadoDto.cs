namespace GFinancas.Application.DTOs.Comum;

/// <summary>
/// Transporta os itens de uma página e os totais necessários à navegação.
/// </summary>
/// <typeparam name="T">Tipo de registro retornado na página.</typeparam>
public class ResultadoPaginadoDto<T>
{
    /// <summary>
    /// Registros retornados na página consultada.
    /// </summary>
    public IEnumerable<T> Itens { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Número da página consultada, com contagem iniciada em 1.
    /// </summary>
    public int PaginaAtual { get; set; }

    /// <summary>
    /// Quantidade de registros solicitada por página.
    /// </summary>
    public int TamanhoPagina { get; set; }

    /// <summary>
    /// Quantidade total de registros encontrados antes da paginação.
    /// </summary>
    public int TotalItens { get; set; }

    /// <summary>
    /// Quantidade de páginas calculada a partir do total de itens e do tamanho da página.
    /// </summary>
    public int TotalPaginas { get; set; }
}