namespace GFinancas.Application.DTOs.Contas;

/// <summary>
/// Apresenta o saldo inicial, as movimentações acumuladas e o saldo atual de uma conta.
/// </summary>
public class SaldoContaResponseDto
{
    /// <summary>
    /// Identificador da conta vinculada à movimentação ou à consulta.
    /// </summary>
    public int ContaId { get; set; }

    /// <summary>
    /// Nome de exibição da conta vinculada.
    /// </summary>
    public string Conta { get; set; } = string.Empty;

    /// <summary>
    /// Valor de partida da conta, anterior às transações registradas.
    /// </summary>
    public decimal SaldoInicial { get; set; }

    /// <summary>
    /// Soma dos valores classificados como receita no conjunto consultado.
    /// </summary>
    public decimal TotalReceitas { get; set; }

    /// <summary>
    /// Soma dos valores classificados como despesa no conjunto consultado.
    /// </summary>
    public decimal TotalDespesas { get; set; }

    /// <summary>
    /// Saldo inicial acrescido das receitas e descontado das despesas.
    /// </summary>
    public decimal SaldoAtual { get; set; }
}