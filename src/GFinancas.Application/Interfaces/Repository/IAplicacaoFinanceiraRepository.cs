using GFinancas.Domain.Entities;

namespace GFinancas.Application.Interfaces.Repositories;

/// <summary>
/// Define a persistência e a consulta de aplicações financeiras por usuário.
/// </summary>
public interface IAplicacaoFinanceiraRepository
{
    /// <summary>
    /// Adiciona o registro ao contexto e salva as alterações no banco de dados.
    /// </summary>
    /// <param name="aplicacao">Aplicação financeira utilizada na operação.</param>
    Task<AplicacaoFinanceira> AdicionarAsync(
        AplicacaoFinanceira aplicacao);

    /// <summary>
    /// Busca a aplicação pelo identificador, restringindo a consulta ao usuário.
    /// </summary>
    /// <param name="id">Identificador da aplicação financeira.</param>
    /// <param name="usuarioId">Identificador do usuário proprietário dos registros.</param>
    Task<AplicacaoFinanceira?> ObterPorIdAsync(
        int id,
        int usuarioId);

    /// <summary>
    /// Lista as aplicações financeiras do usuário.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário proprietário dos registros.</param>
    Task<IEnumerable<AplicacaoFinanceira>> ListarAsync(
        int usuarioId);

    /// <summary>
    /// Atualiza os dados da aplicação financeira.
    /// </summary>
    /// <param name="aplicacao">Aplicação financeira utilizada na operação.</param>
    Task AtualizarAsync(
        AplicacaoFinanceira aplicacao);
}