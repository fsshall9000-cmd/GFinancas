namespace GFinancas.Application.Interfaces.Security;

/// <summary>
/// Define as operações de geração e verificação de hashes de senha.
/// </summary>
public interface ISenhaHasher
{
    /// <summary>
    /// Gera um hash de senha com o formato do ASP.NET Core Identity.
    /// </summary>
    /// <param name="senha">Senha em texto puro recebida para geração ou verificação do hash.</param>
    string GerarHash(string senha);

    /// <summary>
    /// Verifica a senha contra o hash e aceita qualquer resultado diferente de falha.
    /// </summary>
    /// <param name="senha">Senha em texto puro recebida para geração ou verificação do hash.</param>
    /// <param name="senhaHash">Hash armazenado que será comparado à senha informada.</param>
    bool Verificar(string senha, string senhaHash);
}