using GFinancas.Domain.Entities;

namespace GFinancas.Application.Interfaces.Repository;

/// <summary>
/// Define as operações de persistência e consulta de usuários.
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>
    /// Busca usuário pelo identificador e retorna nulo quando não encontra o registro.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    Task<Usuario?> ObterPorIdAsync(int id);

    /// <summary>
    /// Busca o usuário pelo e-mail informado e retorna nulo quando não o encontra.
    /// </summary>
    /// <param name="email">E-mail usado para localizar o usuário.</param>
    Task<Usuario?> ObterPorEmailAsync(string email);

    /// <summary>
    /// Lista todos os registros de usuários, ordenados por nome.
    /// </summary>
    Task<IEnumerable<Usuario>> ObterTodosAsync();

    /// <summary>
    /// Adiciona usuário ao contexto e salva as alterações no banco.
    /// </summary>
    /// <param name="usuario">Usuário utilizado na operação.</param>
    Task AdicionarAsync(Usuario usuario);

    /// <summary>
    /// Troca a senha somente se o hash ainda corresponde ao utilizado para emitir o link.
    /// </summary>
    Task<bool> AtualizarSenhaAsync(int id, string hashAtual, string novoHash);    





}
