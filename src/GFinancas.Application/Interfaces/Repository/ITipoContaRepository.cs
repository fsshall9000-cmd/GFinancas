using GFinancas.Domain.Entities;

namespace GFinancas.Application.Interfaces.Repositories;

/// <summary>
/// Define as operações de persistência e consulta de tipos de conta.
/// </summary>
public interface ITipoContaRepository
{
    /// <summary>
    /// Busca tipo de conta pelo identificador e retorna nulo quando não encontra o registro.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    Task<TipoConta?> ObterPorIdAsync(int id);

    /// <summary>
    /// Lista todos os registros de tipos de conta, ordenados por nome.
    /// </summary>
    Task<IEnumerable<TipoConta>> ObterTodosAsync();
}