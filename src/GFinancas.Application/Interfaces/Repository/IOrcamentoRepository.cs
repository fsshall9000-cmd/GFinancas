using GFinancas.Domain.Entities;

namespace GFinancas.Application.Interfaces.Repositories;

public interface IOrcamentoRepository
{
    Task<Orcamento?> ObterPorIdAsync(
        int id,
        int usuarioId);

    Task<Orcamento?> ObterPorCategoriaMesAsync(
        int usuarioId,
        int categoriaId,
        int ano,
        int mes);

    Task<IEnumerable<Orcamento>> ListarPorMesAsync(
        int usuarioId,
        int ano,
        int mes);

    /// <summary>
    /// Verifica se existe pelo menos um orçamento
    /// associado à categoria informada.
    /// </summary>
    Task<bool> ExistePorCategoriaAsync(
        int categoriaId);

    Task AdicionarAsync(
        Orcamento orcamento);

    Task AtualizarAsync(
        Orcamento orcamento);

    Task RemoverAsync(
        Orcamento orcamento);
}