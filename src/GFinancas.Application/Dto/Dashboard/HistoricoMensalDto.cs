namespace GFinancas.Application.DTOs.Dashboard;

/// <summary>
/// Apresenta os totais financeiros e o resultado de um mês do histórico anual.
/// </summary>
public class HistoricoMensalDto
{
    /// <summary>
    /// Número do mês de referência, de 1 a 12.
    /// </summary>
    public int Mes { get; set; }

    /// <summary>
    /// Nome do mês em português do Brasil.
    /// </summary>
    public string MesNome { get; set; } = string.Empty;

    /// <summary>
    /// Soma dos valores classificados como receita no conjunto consultado.
    /// </summary>
    public decimal TotalReceitas { get; set; }

    /// <summary>
    /// Soma dos valores classificados como despesa no conjunto consultado.
    /// </summary>
    public decimal TotalDespesas { get; set; }

    /// <summary>
    /// Diferença entre as receitas e as despesas do mês.
    /// </summary>
    public decimal Resultado { get; set; }
}