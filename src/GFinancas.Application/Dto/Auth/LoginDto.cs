namespace GFinancas.Application.DTOs.Auth;

/// <summary>
/// Transporta o e-mail e a senha informados para autenticação.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Endereço de e-mail do usuário, usado no cadastro e na autenticação.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Senha informada pelo usuário para cadastro ou autenticação.
    /// </summary>
    public string Senha { get; set; } = string.Empty;
}