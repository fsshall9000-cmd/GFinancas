namespace GFinancas.Application.DTOs.Auth;

/// <summary>
/// Transporta o token, sua expiração e os dados do usuário autenticado.
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// Token JWT emitido para autenticar as próximas requisições.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora de expiração informada para o token, em UTC.
    /// </summary>
    public DateTime Expiracao { get; set; }

    /// <summary>
    /// Identificador do usuário associado ao registro.
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Nome usado para identificar o registro na aplicação.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Endereço de e-mail do usuário, usado no cadastro e na autenticação.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}