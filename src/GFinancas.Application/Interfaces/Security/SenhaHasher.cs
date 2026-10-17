using GFinancas.Application.Interfaces.Security;
using Microsoft.AspNetCore.Identity;

namespace GFinancas.Infrastructure.Security;

/// <summary>
/// Gera e verifica hashes de senha usando o PasswordHasher do ASP.NET Core Identity.
/// </summary>
public class SenhaHasher : ISenhaHasher
{
    /// <summary>
    /// Implementação do ASP.NET Core Identity responsável pelo hash e pela verificação de senhas.
    /// </summary>
    private readonly PasswordHasher<object> _passwordHasher = new();

    /// <summary>
    /// Gera um hash de senha com o formato do ASP.NET Core Identity.
    /// </summary>
    /// <param name="senha">Senha em texto puro recebida para geração ou verificação do hash.</param>
    public string GerarHash(string senha)
    {
        return _passwordHasher.HashPassword(
            null!,
            senha
        );
    }

    /// <summary>
    /// Verifica a senha contra o hash e aceita qualquer resultado diferente de falha.
    /// </summary>
    /// <param name="senha">Senha em texto puro recebida para geração ou verificação do hash.</param>
    /// <param name="senhaHash">Hash armazenado que será comparado à senha informada.</param>
    public bool Verificar(string senha, string senhaHash)
    {
        // Resultado da verificação do hash, usado para identificar falha de autenticação.
        var resultado = _passwordHasher.VerifyHashedPassword(
            null!,
            senhaHash,
            senha
        );

        return resultado != PasswordVerificationResult.Failed;
    }
}