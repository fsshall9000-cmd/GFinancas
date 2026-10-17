using GFinancas.Domain.Entities;

namespace GFinancas.Application.Interfaces.Security;

/// <summary>
/// Define a emissão de tokens de autenticação para usuários.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Gera um JWT assinado com identificador, nome e e-mail do usuário e validade de duas horas.
    /// </summary>
    /// <param name="usuario">Usuário utilizado na operação.</param>
    string GerarToken(Usuario usuario);
}