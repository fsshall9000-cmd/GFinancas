namespace GFinancas.Application.DTOs.Categorias;

/// <summary>
/// Transporta os dados de categoria devolvidos pela aplicação.
/// </summary>
public class CategoriaResponseDto
{
    /// <summary>
    /// Identificador único do registro.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome usado para identificar o registro na aplicação.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Natureza da movimentação: receita (1) ou despesa (2).
    /// </summary>
    public int Tipo { get; set; }

    /// <summary>
    /// Nome textual do tipo de movimentação: Receita ou Despesa.
    /// </summary>
    public string TipoDescricao { get; set; } = string.Empty;

    /// <summary>
    /// Identificador do usuário associado ao registro.
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Nome de exibição do usuário proprietário.
    /// </summary>
    public string Usuario { get; set; } = string.Empty;
}