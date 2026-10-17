using GFinancas.Application.DTOs.Auth;

namespace GFinancas.Application.Interfaces.Services;

/// <summary>
/// Define a autenticação por e-mail e senha.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Valida e-mail e senha, verifica o hash armazenado e retorna o token e os dados do usuário.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
}