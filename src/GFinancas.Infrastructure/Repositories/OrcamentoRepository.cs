using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Domain.Entities;
using GFinancas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GFinancas.Infrastructure.Repositories;

public class OrcamentoRepository : IOrcamentoRepository
{
    private readonly AppDbContext _context;

    public OrcamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Orcamento?> ObterPorIdAsync(int id, int usuarioId)
    {
        return _context.Orcamentos
            .AsNoTracking()
            .Include(o => o.Categoria)
            .FirstOrDefaultAsync(o => o.Id == id && o.UsuarioId == usuarioId);
    }

    public Task<Orcamento?> ObterPorCategoriaMesAsync(
        int usuarioId, int categoriaId, int ano, int mes)
    {
        return _context.Orcamentos
            .AsNoTracking()
            .FirstOrDefaultAsync(o =>
                o.UsuarioId == usuarioId &&
                o.CategoriaId == categoriaId &&
                o.Ano == ano &&
                o.Mes == mes);
    }

    public async Task<IEnumerable<Orcamento>> ListarPorMesAsync(
        int usuarioId, int ano, int mes)
    {
        return await _context.Orcamentos
            .AsNoTracking()
            .Include(o => o.Categoria)
            .Where(o => o.UsuarioId == usuarioId && o.Ano == ano && o.Mes == mes)
            .OrderBy(o => o.Categoria.Nome)
            .ThenBy(o => o.Id)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Orcamento orcamento)
    {
        await _context.Orcamentos.AddAsync(orcamento);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Orcamento orcamento)
    {
        _context.Entry(orcamento).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Orcamento orcamento)
    {
        _context.Entry(orcamento).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistePorCategoriaAsync(int categoriaId)
    {
        return await _context
            .Orcamentos
            .AnyAsync(o =>
            o.CategoriaId == categoriaId);
    }
}
