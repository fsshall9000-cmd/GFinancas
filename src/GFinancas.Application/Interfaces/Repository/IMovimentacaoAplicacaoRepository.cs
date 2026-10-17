using GFinancas.Domain.Entities;

namespace GFinancas.Application.Interfaces.Repositories;

/// <summary>
/// Define a persistência de movimentações e a consulta de históricos e totais.
/// </summary>
public interface IMovimentacaoAplicacaoRepository
{
    /// <summary>
    /// Adiciona o registro ao contexto e salva as alterações no banco de dados.
    /// </summary>
    /// <param name="movimentacao">Movimentação de aplicação financeira utilizada na operação.</param>
    Task<MovimentacaoAplicacao> AdicionarAsync(
        MovimentacaoAplicacao movimentacao);

    /// <summary>
    /// Lista as movimentações da aplicação vinculada ao usuário informado.
    /// </summary>
    /// <param name="aplicacaoId">Identificador da aplicação financeira.</param>
    /// <param name="usuarioId">Identificador do usuário proprietário dos registros.</param>
    Task<IEnumerable<MovimentacaoAplicacao>>
        ListarPorAplicacaoAsync(
            int aplicacaoId, int usuarioId);

    /// <summary>
    /// Soma os aportes registrados para a aplicação informada.
    /// </summary>
    /// <param name="aplicacaoId">Identificador da aplicação financeira.</param>
    Task<decimal> ObterTotalAportesAsync(
        int aplicacaoId);

    /// <summary>
    /// Soma os resgates registrados para a aplicação informada.
    /// </summary>
    /// <param name="aplicacaoId">Identificador da aplicação financeira.</param>
    Task<decimal> ObterTotalResgatesAsync(
        int aplicacaoId);

    /// <summary>
    /// Soma os aportes que saíram da conta informada.
    /// </summary>
    /// <param name="contaId">Identificador da conta financeira.</param>
    Task<decimal> ObterTotalAportesPorContaAsync(
        int contaId);

    /// <summary>
    /// Soma os resgates que entraram na conta informada.
    /// </summary>
    /// <param name="contaId">Identificador da conta financeira.</param>
    Task<decimal> ObterTotalResgatesPorContaAsync(
        int contaId);

    /// <summary>
    /// Soma os aportes que saíram da conta informada até a data limite.
    /// </summary>
    /// <param name="contaId"></param>
    /// <param name="dataLimite"></param>
    /// <returns></returns>
    Task<decimal> ObterTotalAportesPorContaAteDataAsync(int contaId,DateTime dataLimite);

    /// <summary>
    /// Soma os resgates que entraram na conta informada até a data limite.
    /// </summary>
    /// <param name="contaId"></param>
    /// <param name="dataLimite"></param>
    /// <returns></returns>
    Task<decimal> ObterTotalResgatesPorContaAteDataAsync(int contaId,DateTime dataLimite);

    /// <summary>
    /// Verifica se existem movimentações vinculadas à conta informada.
    /// </summary>
    /// <param name="contaId"></param>
    /// <returns></returns>
    Task<bool> ExistePorContaAsync(int contaId);

    /// <summary>
    /// Lista as movimentações da aplicação vinculada ao usuário informado até a data limite.
    /// </summary>
    /// <param name="aplicacaoId"></param>
    /// <param name="usuarioId"></param>
    /// <param name="dataLimite"></param>
    /// <returns></returns>
    Task<IEnumerable<MovimentacaoAplicacao>> ListarAteDataAsync(int aplicacaoId,int usuarioId,DateTime dataLimite);

    /// <summary>
    /// Lista as movimentações da aplicação vinculada ao usuário informado em ordem cronológica.
    /// </summary>
    /// <param name="aplicacaoId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    Task<IEnumerable<MovimentacaoAplicacao>> ListarCronologicamenteAsync(int aplicacaoId,int usuarioId);
}