namespace GFinancas.Domain.Enums;

/// <summary>
/// Distingue entradas de dinheiro (receitas) de saídas (despesas).
/// </summary>
public enum TipoTransacao
{
	/// <summary>
	/// Entrada de dinheiro, somada ao saldo da conta.
	/// </summary>
	Receita = 1,
	/// <summary>
	/// Saída de dinheiro, descontada do saldo da conta.
	/// </summary>
	Despesa = 2
}
