namespace GFinancas.Application.DTOs.TiposConta;

/// <summary>
/// Representa os dados de um tipo de conta retornados pela aplicação.
/// </summary>
public class TipoContaResponseDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
}