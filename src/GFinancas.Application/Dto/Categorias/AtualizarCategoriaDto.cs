using GFinancas.Domain.Enums;

namespace GFinancas.Application.DTOs.Categorias;

/// <summary>
/// Transporta os dados necessários para atualizar uma categoria.
/// </summary>
public class AtualizarCategoriaDto
{
    public string Nome { get; set; } = string.Empty;

    public TipoTransacao Tipo { get; set; }
}
