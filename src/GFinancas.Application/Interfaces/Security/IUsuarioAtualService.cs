namespace GFinancas.Application.Interfaces.Security;

/// <summary>
/// Define o acesso ao identificador do usuário da requisição atual.
/// </summary>
public interface IUsuarioAtualService
{
    /// <summary>
    /// Identificador do usuário obtido da requisição atual; a implementação rejeita declarações ausentes ou inválidas.
    /// </summary>
    int UsuarioId { get; }
}