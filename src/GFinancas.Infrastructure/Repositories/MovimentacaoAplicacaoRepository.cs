using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Domain.Entities;
using GFinancas.Domain.Enums;
using GFinancas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GFinancas.Infrastructure.Repositories;

/// <summary>
/// Persiste movimentações de aplicações e consulta históricos e totais de aportes e resgates.
/// </summary>
public class MovimentacaoAplicacaoRepository
    : IMovimentacaoAplicacaoRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Recebe e armazena as dependências utilizadas pelo componente.
    /// </summary>
    /// <param name="context">Contexto de banco de dados compartilhado pelos repositórios.</param>
    public MovimentacaoAplicacaoRepository(
        AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona o registro ao contexto e salva as alterações no banco de dados.
    /// </summary>
    /// <param name="movimentacao">Movimentação de aplicação financeira utilizada na operação.</param>
    public async Task<MovimentacaoAplicacao> AdicionarAsync(
        MovimentacaoAplicacao movimentacao)
    {
        await _context.MovimentacoesAplicacao
            .AddAsync(movimentacao);

        await _context.SaveChangesAsync();

        return movimentacao;
    }

    /// <summary>
    /// Lista as movimentações da aplicação vinculada ao usuário informado.
    /// </summary>
    /// <param name="aplicacaoId">Identificador da aplicação financeira.</param>
    /// <param name="usuarioId">Identificador do usuário proprietário dos registros.</param>
    public async Task<IEnumerable<MovimentacaoAplicacao>>
    ListarPorAplicacaoAsync(
        int aplicacaoId,
        int usuarioId)
{
    return await _context
    .MovimentacoesAplicacao
    .AsNoTracking()
    .Include(m => m.Conta)
    .Where(m =>
        m.AplicacaoFinanceiraId == aplicacaoId &&
        m.AplicacaoFinanceira.UsuarioId == usuarioId)
    .OrderByDescending(m => m.Data)
    .ThenByDescending(m => m.Id)
    .ToListAsync();
}

    /// <summary>
    /// Soma os aportes registrados para a aplicação informada.
    /// </summary>
    /// <param name="aplicacaoId">Identificador da aplicação financeira.</param>
    public async Task<decimal> ObterTotalAportesAsync(
        int aplicacaoId)
    {
        return await _context.MovimentacoesAplicacao
            .AsNoTracking()
            .Where(m =>
                m.AplicacaoFinanceiraId == aplicacaoId &&
                m.Tipo == TipoMovimentacaoAplicacao.Aporte)
            .SumAsync(m => m.Valor);
    }

    /// <summary>
    /// Soma os resgates registrados para a aplicação informada.
    /// </summary>
    /// <param name="aplicacaoId">Identificador da aplicação financeira.</param>
    public async Task<decimal> ObterTotalResgatesAsync(
        int aplicacaoId)
    {
        return await _context.MovimentacoesAplicacao
            .AsNoTracking()
            .Where(m =>
                m.AplicacaoFinanceiraId == aplicacaoId &&
                m.Tipo == TipoMovimentacaoAplicacao.Resgate)
            .SumAsync(m => m.Valor);
    }

    /// <summary>
    /// Soma os aportes que saíram da conta informada.
    /// </summary>
    /// <param name="contaId">Identificador da conta financeira.</param>
    public async Task<decimal> ObterTotalAportesPorContaAsync(
        int contaId)
    {
        return await _context.MovimentacoesAplicacao
            .AsNoTracking()
            .Where(m =>
                m.ContaId == contaId &&
                m.Tipo == TipoMovimentacaoAplicacao.Aporte)
            .SumAsync(m => m.Valor);
    }

    /// <summary>
    /// Soma os resgates que entraram na conta informada.
    /// </summary>
    /// <param name="contaId">Identificador da conta financeira.</param>
    public async Task<decimal> ObterTotalResgatesPorContaAsync(
        int contaId)
    {
        return await _context.MovimentacoesAplicacao
            .AsNoTracking()
            .Where(m =>
                m.ContaId == contaId &&
                m.Tipo == TipoMovimentacaoAplicacao.Resgate)
            .SumAsync(m => m.Valor);
    }

    /// <summary>
    /// Soma os aportes que saíram da conta informada até a data limite.
    /// </summary>
    /// <param name="contaId"></param>
    /// <param name="dataLimite"></param>
    /// <returns></returns>
    public async Task<decimal>
    ObterTotalAportesPorContaAteDataAsync(
        int contaId,
        DateTime dataLimite)
    {
    return await _context.MovimentacoesAplicacao
        .Where(m =>
            m.ContaId == contaId &&
            m.Tipo == TipoMovimentacaoAplicacao.Aporte &&
            m.Data <= dataLimite)
        .SumAsync(m => m.Valor);
    }

    public async Task<decimal>
    ObterTotalResgatesPorContaAteDataAsync(
        int contaId,
        DateTime dataLimite)
    {
    return await _context.MovimentacoesAplicacao
        .Where(m =>
            m.ContaId == contaId &&
            m.Tipo == TipoMovimentacaoAplicacao.Resgate &&
            m.Data <= dataLimite)
        .SumAsync(m => m.Valor);
    }

    /// <summary>
/// Lista as movimentações de uma aplicação até
/// determinada data e horário, em ordem cronológica.
/// </summary>
public async Task<IEnumerable<MovimentacaoAplicacao>>
    ListarAteDataAsync(
        int aplicacaoId,
        int usuarioId,
        DateTime dataLimite)
{
    return await _context
        .MovimentacoesAplicacao
        .AsNoTracking()
        .Where(m =>
            m.AplicacaoFinanceiraId == aplicacaoId &&
            m.AplicacaoFinanceira.UsuarioId == usuarioId &&
            m.Data <= dataLimite)
        .OrderBy(m => m.Data)
        .ThenBy(m => m.Id)
        .ToListAsync();
}

    /// <summary>
    /// Verifica se existem movimentações vinculadas à conta informada.
    /// </summary>
    /// <param name="contaId"></param>
    /// <returns></returns>
    public async Task<bool> ExistePorContaAsync(int contaId)
    {
        return await _context
            .MovimentacoesAplicacao
            .AnyAsync(m => m.ContaId == contaId);
    }

    /// <summary>
    /// Lista as movimentações da aplicação vinculada ao usuário informado em ordem cronológica.
    /// </summary>
    /// <param name="aplicacaoId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<MovimentacaoAplicacao>>
    ListarCronologicamenteAsync(
        int aplicacaoId,
        int usuarioId)
    {
        return await _context.MovimentacoesAplicacao
            .AsNoTracking()
            .Where(m =>
            m.AplicacaoFinanceiraId == aplicacaoId &&
            m.AplicacaoFinanceira.UsuarioId == usuarioId)
            .OrderBy(m => m.Data)
            .ThenBy(m => m.Id)
            .ToListAsync();
    }
}