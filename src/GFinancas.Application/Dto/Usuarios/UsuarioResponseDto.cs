namespace GFinancas.Application.Dto.Usuarios;

/// <summary>
/// Transporta os dados de usuário devolvidos pela aplicação.
/// </summary>
public class UsuarioResponseDto
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
    /// Endereço de e-mail do usuário, usado no cadastro e na autenticação.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}
