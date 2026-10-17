using GFinancas.Domain.Enums;

namespace GFinancas.Domain.Entities;

/// <summary>
/// Representa uma movimentação financeira vinculada a uma conta e a uma categoria.
/// </summary>
public class Transacao
{
    /// <summary>
    /// Identificador único do registro.
    /// </summary>
    public int Id { get; set; }

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

    // FK Conta
    /// <summary>
    /// Identificador da conta vinculada à movimentação ou à consulta.
    /// </summary>
    public int ContaId { get; set; }
    /// <summary>
    /// Propriedade de navegação para os dados da conta vinculada.
    /// </summary>
    public Conta? Conta { get; set; }

    // FK Categoria
    /// <summary>
    /// Identificador da categoria usada para classificar a movimentação.
    /// </summary>
    public int CategoriaId { get; set; }
    /// <summary>
    /// Propriedade de navegação para os dados da categoria vinculada.
    /// </summary>
    public Categoria? Categoria { get; set; }

}
