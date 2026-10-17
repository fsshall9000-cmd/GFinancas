using GFinancas.Domain.Entities;

namespace GFinancas.Application.Interfaces.Repositories;

public interface IHistoricoValorAplicacaoRepository
{
    Task<HistoricoValorAplicacao?> ObterPorAplicacaoEDataAsync(
        int aplicacaoFinanceiraId,
        DateOnly dataReferencia);

    Task<HistoricoValorAplicacao> AdicionarAsync(
        HistoricoValorAplicacao historico);

    Task AtualizarAsync(
        HistoricoValorAplicacao historico);

    Task<IEnumerable<HistoricoValorAplicacao>> ListarPorAplicacaoAsync(
        int aplicacaoFinanceiraId,
        int usuarioId);

    Task<HistoricoValorAplicacao?> ObterUltimoAteDataAsync(
        int aplicacaoFinanceiraId,
        int usuarioId,
        DateOnly dataReferencia);

    Task<HistoricoValorAplicacao?> ObterMaisRecenteAsync(
        int aplicacaoFinanceiraId);

    Task<IEnumerable<HistoricoValorAplicacao>> ListarAPartirDaDataAsync(
        int aplicacaoFinanceiraId,
        DateOnly dataReferencia);

    Task<HistoricoValorAplicacao?> ObterUltimoAntesDaDataAsync(
        int aplicacaoFinanceiraId,
        DateOnly dataReferencia);
}