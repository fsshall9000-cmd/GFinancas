namespace GFinancas.Application.DTOs.Dashboard;

/// <summary>
/// Apresenta o total e a participação percentual das despesas de uma categoria.
/// </summary>
public class DespesaPorCategoriaDto
{
    /// <summary>
    /// Identificador da categoria usada para classificar a movimentação.
    /// </summary>
    public int CategoriaId { get; set; }

    /// <summary>
    /// Nome de exibição da categoria vinculada.
    /// </summary>
    public string Categoria { get; set; } = string.Empty;

    /// <summary>
    /// Soma das despesas da categoria no período consultado.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Participação da categoria no total de despesas do período, em porcentagem.
    /// </summary>
    public decimal Percentual { get; set; }
}