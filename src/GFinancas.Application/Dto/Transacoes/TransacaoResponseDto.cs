namespace GFinancas.Application.Dto.Transacoes;

/// <summary>
/// Transporta os dados de transação devolvidos pela aplicação.
/// </summary>
public class TransacaoResponseDto
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
    public int Tipo { get; set; }

    /// <summary>
    /// Nome textual do tipo de movimentação: Receita ou Despesa.
    /// </summary>
    public string TipoDescricao { get; set; } = string.Empty;

    /// <summary>
    /// Identificador da conta vinculada à movimentação ou à consulta.
    /// </summary>
    public int ContaId { get; set; }

    /// <summary>
    /// Nome de exibição da conta vinculada.
    /// </summary>
    public string Conta { get; set; } = string.Empty;

    /// <summary>
    /// Identificador da categoria usada para classificar a movimentação.
    /// </summary>
    public int CategoriaId { get; set; }

    /// <summary>
    /// Nome de exibição da categoria vinculada.
    /// </summary>
    public string Categoria { get; set; } = string.Empty;



}
