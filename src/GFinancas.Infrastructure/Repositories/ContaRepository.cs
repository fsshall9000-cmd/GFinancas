using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Domain.Entities;
using GFinancas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GFinancas.Infrastructure.Repositories;

/// <summary>
/// Consulta e persiste dados de contas usando o Entity Framework Core.
/// </summary>
public class ContaRepository : IContaRepository
{
    /// <summary>
    /// Contexto do Entity Framework Core usado para consultar e salvar entidades.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="context">Contexto do Entity Framework Core usado para consultar e salvar entidades.</param>
    public ContaRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona conta ao contexto e salva as alterações no banco.
    /// </summary>
    /// <param name="conta">Conta financeira utilizada na operação.</param>
    public async Task AdicionarAsync(Conta conta)
    {
        await _context.Contas.AddAsync(conta);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Busca conta pelo identificador e retorna nulo quando não encontra o registro.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>c</c> representa a conta na expressão de consulta ou mapeamento.
    /// </remarks>
    public async Task<Conta?> ObterPorIdAsync(int id)
    {
        return await _context.Contas
            .AsNoTracking()
            .Include(c => c.Usuario)
            .Include(c => c.TipoConta)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Atualiza a conta e salva as alterações no banco.
    /// </summary>
    /// <param name="conta">Conta financeira utilizada na operação.</param>
    public async Task AtualizarAsync(Conta conta)
    {
        _context.Entry(conta).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Remove a conta e salva as alterações no banco.
    /// </summary>
    /// <param name="conta">Conta financeira utilizada na operação.</param>
    public async Task RemoverAsync(Conta conta)
    {
        _context.Entry(conta).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Lista contas do usuário informado, em ordem de nome.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário cujos dados serão consultados.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>c</c> representa a conta na expressão de consulta ou mapeamento.
    /// </remarks>
    public async Task<IEnumerable<Conta>> ObterPorUsuarioIdAsync(
    int usuarioId)
{
    return await _context.Contas
        .AsNoTracking()
        .Include(c => c.TipoConta)
        .Where(c => c.UsuarioId == usuarioId)
        .OrderBy(c => c.Nome)
        .ToListAsync();
}

    /// <summary>
    /// Lista todos os registros de contas, ordenados por nome.
    /// </summary>
    /// <remarks>
    /// Nas expressões lambda, <c>c</c> representa a conta na expressão de consulta ou mapeamento.
    /// </remarks>
    public async Task<IEnumerable<Conta>> ObterTodasAsync()
    {
        return await _context.Contas
            .AsNoTracking()
            .Include(c => c.Usuario)
            .Include(c => c.TipoConta)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }
}
