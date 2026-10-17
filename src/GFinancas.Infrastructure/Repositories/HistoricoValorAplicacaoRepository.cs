using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Domain.Entities;
using GFinancas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GFinancas.Infrastructure.Repositories;

public class HistoricoValorAplicacaoRepository
    : IHistoricoValorAplicacaoRepository
{
    private readonly AppDbContext _context;

    public HistoricoValorAplicacaoRepository(
        AppDbContext context)
    {
        _context = context;
    }

    // =====================================================
    // BUSCAR HISTÓRICO DE UMA APLICAÇÃO EM UMA DATA
    // =====================================================

    public async Task<HistoricoValorAplicacao?>
        ObterPorAplicacaoEDataAsync(
            int aplicacaoFinanceiraId,
            DateOnly dataReferencia)
    {
        return await _context.HistoricosValoresAplicacoes
            .FirstOrDefaultAsync(h =>
                h.AplicacaoFinanceiraId ==
                    aplicacaoFinanceiraId &&
                h.DataReferencia ==
                    dataReferencia);
    }

    // =====================================================
    // ADICIONAR
    // =====================================================

    public async Task<HistoricoValorAplicacao>
        AdicionarAsync(
            HistoricoValorAplicacao historico)
    {
        await _context.HistoricosValoresAplicacoes
            .AddAsync(historico);

        await _context.SaveChangesAsync();

        return historico;
    }

    // =====================================================
    // ATUALIZAR
    // =====================================================

    public async Task AtualizarAsync(
        HistoricoValorAplicacao historico)
    {
        _context.HistoricosValoresAplicacoes
            .Update(historico);

        await _context.SaveChangesAsync();
    }

    // =====================================================
    // LISTAR HISTÓRICO DA APLICAÇÃO
    // =====================================================

    public async Task<IEnumerable<HistoricoValorAplicacao>>
        ListarPorAplicacaoAsync(
            int aplicacaoFinanceiraId,
            int usuarioId)
    {
        return await _context.HistoricosValoresAplicacoes
            .AsNoTracking()
            .Where(h =>
                h.AplicacaoFinanceiraId ==
                    aplicacaoFinanceiraId &&
                h.AplicacaoFinanceira.UsuarioId ==
                    usuarioId)
            .OrderBy(h => h.DataReferencia)
            .ToListAsync();
    }

    // =====================================================
    // BUSCAR ÚLTIMO VALOR CONHECIDO ATÉ UMA DATA
    // COM VALIDAÇÃO DO PROPRIETÁRIO
    // =====================================================

    public async Task<HistoricoValorAplicacao?>
        ObterUltimoAteDataAsync(
            int aplicacaoFinanceiraId,
            int usuarioId,
            DateOnly dataReferencia)
    {
        return await _context.HistoricosValoresAplicacoes
            .AsNoTracking()
            .Where(h =>
                h.AplicacaoFinanceiraId ==
                    aplicacaoFinanceiraId &&
                h.AplicacaoFinanceira.UsuarioId ==
                    usuarioId &&
                h.DataReferencia <=
                    dataReferencia)
            .OrderByDescending(h => h.DataReferencia)
            .FirstOrDefaultAsync();
    }

    // =====================================================
    // BUSCAR O REGISTRO MAIS RECENTE
    // =====================================================

    public async Task<HistoricoValorAplicacao?>
        ObterMaisRecenteAsync(
            int aplicacaoFinanceiraId)
    {
        return await _context.HistoricosValoresAplicacoes
            .AsNoTracking()
            .Where(h =>
                h.AplicacaoFinanceiraId ==
                    aplicacaoFinanceiraId)
            .OrderByDescending(h => h.DataReferencia)
            .FirstOrDefaultAsync();
    }

    // =====================================================
    // LISTAR HISTÓRICOS A PARTIR DE UMA DATA
    // =====================================================

    public async Task<IEnumerable<HistoricoValorAplicacao>>
        ListarAPartirDaDataAsync(
            int aplicacaoFinanceiraId,
            DateOnly dataReferencia)
    {
        return await _context.HistoricosValoresAplicacoes
            .Where(h =>
                h.AplicacaoFinanceiraId ==
                    aplicacaoFinanceiraId &&
                h.DataReferencia >=
                    dataReferencia)
            .OrderBy(h => h.DataReferencia)
            .ToListAsync();
    }

    // =====================================================
    // BUSCAR ÚLTIMO HISTÓRICO ANTERIOR À DATA
    // =====================================================

    public async Task<HistoricoValorAplicacao?>
        ObterUltimoAntesDaDataAsync(
            int aplicacaoFinanceiraId,
            DateOnly dataReferencia)
    {
        return await _context.HistoricosValoresAplicacoes
            .Where(h =>
                h.AplicacaoFinanceiraId ==
                    aplicacaoFinanceiraId &&
                h.DataReferencia <
                    dataReferencia)
            .OrderByDescending(h => h.DataReferencia)
            .FirstOrDefaultAsync();
    }

    // =====================================================
    // BUSCAR ÚLTIMO HISTÓRICO ATÉ A DATA
    // SEM PARÂMETRO DE USUÁRIO
    //
    // ATENÇÃO:
    // Usar apenas após verificar a propriedade
    // da aplicação no serviço.
    // =====================================================

    public async Task<HistoricoValorAplicacao?>
        ObterUltimoAteDataAsync(
            int aplicacaoFinanceiraId,
            DateOnly dataReferencia)
    {
        return await _context.HistoricosValoresAplicacoes
            .AsNoTracking()
            .Where(h =>
                h.AplicacaoFinanceiraId ==
                    aplicacaoFinanceiraId &&
                h.DataReferencia <=
                    dataReferencia)
            .OrderByDescending(h => h.DataReferencia)
            .FirstOrDefaultAsync();
    }
}