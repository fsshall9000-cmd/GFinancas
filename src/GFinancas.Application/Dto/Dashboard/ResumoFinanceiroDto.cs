namespace GFinancas.Application.DTOs.Dashboard;

/// <summary>
/// Apresenta receitas, despesas, resultado e quantidade de transações de um mês.
/// </summary>
public class ResumoFinanceiroDto
{
    /// <summary>
    /// Ano de referência do resumo financeiro.
    /// </summary>
    public int Ano { get; set; }

    /// <summary>
    /// Número do mês de referência, de 1 a 12.
    /// </summary>
    public int Mes { get; set; }

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
    public decimal ResultadoMes { get; set; }

    /// <summary>
    /// Quantidade de movimentações incluídas no resumo mensal.
    /// </summary>
    public int QuantidadeTransacoes { get; set; }
}