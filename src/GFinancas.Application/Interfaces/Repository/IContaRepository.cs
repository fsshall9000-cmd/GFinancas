using GFinancas.Domain.Entities;

namespace GFinancas.Application.Interfaces.Repositories;

/// <summary>
/// Define as operações de persistência e consulta de contas.
/// </summary>
public interface IContaRepository
{
    /// <summary>
    /// Adiciona conta ao contexto e salva as alterações no banco.
    /// </summary>
    /// <param name="conta">Conta financeira utilizada na operação.</param>
    Task AdicionarAsync(Conta conta);

    /// <summary>
    /// Atualiza a conta e salva as alterações no banco.
    /// </summary>
    /// <param name="conta">Conta financeira utilizada na operação.</param>
    Task AtualizarAsync(Conta conta);

    /// <summary>
    /// Remove a conta e salva as alterações no banco.
    /// </summary>
    /// <param name="conta">Conta financeira utilizada na operação.</param>
    Task RemoverAsync(Conta conta);

    /// <summary>
    /// Busca conta pelo identificador e retorna nulo quando não encontra o registro.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    Task<Conta?> ObterPorIdAsync(int id);

    /// <summary>
    /// Lista todos os registros de contas, ordenados por nome.
    /// </summary>
    Task<IEnumerable<Conta>> ObterTodasAsync();

    /// <summary>
    /// Lista contas do usuário informado, em ordem de nome.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário cujos dados serão consultados.</param>
    Task<IEnumerable<Conta>> ObterPorUsuarioIdAsync(int usuarioId);
}
