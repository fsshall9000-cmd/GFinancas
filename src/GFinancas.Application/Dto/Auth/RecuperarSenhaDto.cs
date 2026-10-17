using System.ComponentModel.DataAnnotations;

namespace GFinancas.Application.DTOs.Auth;

public sealed class RecuperarSenhaDto
{
    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = string.Empty;
}

public sealed class RedefinirSenhaDto
{
    [Required, MaxLength(4096)]
    public string Token { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(128)]
    public string Senha { get; set; } = string.Empty;
}
