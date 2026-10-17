using GFinancas.Application.Interfaces.Repository;
using GFinancas.Domain.Entities;
using GFinancas.Domain.Enums;
using GFinancas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GFinancas.Infrastructure.Repositories;

/// <summary>
/// Consulta e persiste dados de transações usando o Entity Framework Core.
/// </summary>
public class TransacaoRepository : ITransacaoRepository
{
    /// <summary>
    /// Contexto do Entity Framework Core usado para consultar e salvar entidades.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="context">Contexto do Entity Framework Core usado para consultar e salvar entidades.</param>
    public TransacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona transação ao contexto e salva as alterações no banco.
    /// </summary>
    /// <param name="transacao">Transação utilizada na operação.</param>
    public async Task AdicionarAsync(Transacao transacao)
    {
        await _context.Transacoes.AddAsync(transacao);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> ObterTotalDespesasPorCategoriaAsync(
    int usuarioId,
    int categoriaId,
    DateTime dataInicio,
    DateTime dataFim)
{
    return await _context.Transacoes
        .AsNoTracking()
        .Where(t =>
            t.Conta.UsuarioId == usuarioId &&
            t.CategoriaId == categoriaId &&
            t.Tipo == TipoTransacao.Despesa &&
            t.Data >= dataInicio &&
            t.Data < dataFim
        )
        .SumAsync(t => t.Valor);
}

    /// <summary>
    /// Busca transação pelo identificador e retorna nulo quando não encontra o registro.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>t</c> representa a transação na expressão de consulta, cálculo ou projeção.
    /// </remarks>
    public async Task<Transacao?> ObterPorIdAsync(int id)
    {
        return await _context.Transacoes
            .AsNoTracking()
            .Include(t => t.Conta)
            .Include(t => t.Categoria)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    /// <summary>
    /// Lista todos os registros de transações, ordenados por data decrescente.
    /// </summary>
    /// <remarks>
    /// Nas expressões lambda, <c>t</c> representa a transação na expressão de consulta, cálculo ou projeção.
    /// </remarks>
    public async Task<IEnumerable<Transacao>> ObterTodasAsync()
    {
        return await _context.Transacoes
            .AsNoTracking()
            .Include(t => t.Conta)
            .Include(t => t.Categoria)
            .OrderByDescending(t => t.Data)
            .ToListAsync();
    }

    /// <summary>
    /// Atualiza a transação no contexto e salva as alterações no banco.
    /// </summary>
    /// <param name="transacao">Transação utilizada na operação.</param>
    public async Task AtualizarAsync(Transacao transacao)
    {
        _context.Transacoes.Update(transacao);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Remove a transação do contexto e salva a exclusão no banco.
    /// </summary>
    /// <param name="transacao">Transação utilizada na operação.</param>
    public async Task RemoverAsync(Transacao transacao)
    {
        _context.Transacoes.Remove(transacao);
        await _context.SaveChangesAsync();
    }

    
    /// <summary>
    /// Lista as transações vinculadas à conta informada.
    /// </summary>
    /// <param name="contaId">Identificador da conta financeira.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>t</c> representa a transação na expressão de consulta, cálculo ou projeção.
    /// </remarks>
    public async Task<IEnumerable<Transacao>> ObterPorContaIdAsync(int contaId)
    {
          return await _context.Transacoes
            .AsNoTracking()
            .Where(t => t.ContaId == contaId)
            .ToListAsync();
    }

    /// <summary>
    /// Consulta transações do usuário informado, aplicando os filtros opcionais e ordenando por data decrescente.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário cujos dados serão consultados.</param>
    /// <param name="dataInicio">Limite inicial inclusivo da consulta; nulo para não aplicar este limite.</param>
    /// <param name="dataFim">Limite final inclusivo da consulta; nulo para não aplicar este limite.</param>
    /// <param name="tipo">Tipo de movimentação: receita ou despesa.</param>
    /// <param name="contaId">Identificador da conta financeira.</param>
    /// <param name="categoriaId">Identificador da categoria.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>t</c> representa a transação na expressão de consulta, cálculo ou projeção.
    /// </remarks>
    public async Task<IEnumerable<Transacao>> FiltrarAsync(
    int usuarioId,
    DateTime? dataInicio,
    DateTime? dataFim,
    TipoTransacao? tipo,
    int? contaId,
    int? categoriaId)
{
    // Consulta de transações restrita ao usuário, ainda não executada no banco.
    var query = _context.Transacoes
        .AsNoTracking()
        .Include(t => t.Conta)
        .Include(t => t.Categoria)
        .Where(t => t.Conta!.UsuarioId == usuarioId)
        .AsQueryable();

    if (dataInicio.HasValue)
    {
        query = query.Where(
            t => t.Data >= dataInicio.Value);
    }

    if (dataFim.HasValue)
    {
        query = query.Where(
            t => t.Data <= dataFim.Value);
    }

    if (tipo.HasValue)
    {
        query = query.Where(
            t => t.Tipo == tipo.Value);
    }

    if (contaId.HasValue)
    {
        query = query.Where(
            t => t.ContaId == contaId.Value);
    }

    if (categoriaId.HasValue)
    {
        query = query.Where(
            t => t.CategoriaId == categoriaId.Value);
    }

    return await query
        .OrderByDescending(t => t.Data)
        .ToListAsync();
    }

    /// <summary>
    /// Retorna uma página de transações do usuário e o total de registros da consulta.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário cujos dados serão consultados.</param>
    /// <param name="pagina">Número da página solicitada, começando em 1.</param>
    /// <param name="tamanhoPagina">Quantidade de itens solicitada por página.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>t</c> representa a transação na expressão de consulta, cálculo ou projeção.
    /// </remarks>
    public async Task<(IEnumerable<Transacao> Itens, int TotalItens)>
    ObterPaginadoAsync(
        int usuarioId,
        int pagina,
        int tamanhoPagina)
{
    // Consulta de transações restrita ao usuário, ainda não executada no banco.
    var query = _context.Transacoes
        .AsNoTracking()
        .Include(t => t.Conta)
        .Include(t => t.Categoria)
        .Where(t => t.Conta!.UsuarioId == usuarioId);

    // Quantidade de transações encontradas antes de aplicar a paginação.
    var totalItens =
        await query.CountAsync();

    // Transações da página, ordenadas por data e identificador decrescentes.
    var itens = await query
        .OrderByDescending(t => t.Data)
        .ThenByDescending(t => t.Id)
        .Skip((pagina - 1) * tamanhoPagina)
        .Take(tamanhoPagina)
        .ToListAsync();

    return (itens, totalItens);
}
    public async Task<decimal> ObterTotalReceitasAteDataAsync(int contaId,DateTime dataLimite)
    {
    return await _context.Transacoes
        .Where(t =>
            t.ContaId == contaId &&
            t.Tipo == TipoTransacao.Receita &&
            t.Data <= dataLimite)
        .SumAsync(t => t.Valor);
    }

    public async Task<decimal> ObterTotalDespesasAteDataAsync(int contaId, DateTime dataLimite)
    {
        return await _context.Transacoes
            .Where(t =>
                t.ContaId == contaId &&
                t.Tipo == TipoTransacao.Despesa &&
                t.Data <= dataLimite)
            .SumAsync(t => t.Valor);
    }

}