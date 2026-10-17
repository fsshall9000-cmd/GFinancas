using GFinancas.Domain.Enums;

namespace GFinancas.Application.DTOs.Categorias;

/// <summary>
/// Transporta os dados necessários para cadastrar categoria.
/// </summary>
public class CriarCategoriaDto
{
    /// <summary>
    /// Nome usado para identificar o registro na aplicação.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Natureza da movimentação: receita (1) ou despesa (2).
    /// </summary>
    public TipoTransacao Tipo { get; set; }

    
}