using GFinancas.Domain.Enums;

namespace GFinancas.Application.Dto.Transacoes;

/// <summary>
/// Transporta os dados necessários para cadastrar transação.
/// </summary>
public sealed class CriarTransacaoDto
{
 	/// <summary>
 	/// Descrição da movimentação financeira.
 	/// </summary>
 	public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Valor monetário da transação; o tipo indica se é receita ou despesa.
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Data da movimentação financeira.
    /// </summary>
    public DateTime Data { get; set; }

    /// <summary>
    /// Natureza da movimentação: receita (1) ou despesa (2).
    /// </summary>
    public TipoTransacao Tipo { get; set; }

    /// <summary>
    /// Identificador da conta vinculada à movimentação ou à consulta.
    /// </summary>
    public int ContaId { get; set; }

    /// <summary>
    /// Identificador da categoria usada para classificar a movimentação.
    /// </summary>
    public int CategoriaId { get; set; }
}
