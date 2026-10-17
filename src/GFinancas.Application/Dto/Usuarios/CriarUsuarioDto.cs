namespace GFinancas.Application.Dto.Usuarios;

/// <summary>
/// Transporta os dados necessários para cadastrar usuário.
/// </summary>
public sealed class CriarUsuarioDto
{
	/// <summary>
	/// Nome usado para identificar o registro na aplicação.
	/// </summary>
	public string Nome { get; set; } = string.Empty;
	/// <summary>
	/// Endereço de e-mail do usuário, usado no cadastro e na autenticação.
	/// </summary>
	public string Email { get; set; } = string.Empty;
	/// <summary>
	/// Senha informada pelo usuário para cadastro ou autenticação.
	/// </summary>
	public string Senha { get; set; } = string.Empty;

	
}
