using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Domain.Entities;
using GFinancas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GFinancas.Infrastructure.Repositories;

/// <summary>
/// Consulta e persiste dados de tipos de conta usando o Entity Framework Core.
/// </summary>
public class TipoContaRepository : ITipoContaRepository
{
    /// <summary>
    /// Contexto do Entity Framework Core usado para consultar e salvar entidades.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="context">Contexto do Entity Framework Core usado para consultar e salvar entidades.</param>
    public TipoContaRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca tipo de conta pelo identificador e retorna nulo quando não encontra o registro.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>t</c> representa o tipo de conta na expressão de consulta ou mapeamento.
    /// </remarks>
    public async Task<TipoConta?> ObterPorIdAsync(int id)
    {
        return await _context.TiposConta
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    /// <summary>
    /// Lista todos os registros de tipos de conta, ordenados por nome.
    /// </summary>
    /// <remarks>
    /// Nas expressões lambda, <c>t</c> representa o tipo de conta na expressão de consulta ou mapeamento.
    /// </remarks>
    public async Task<IEnumerable<TipoConta>> ObterTodosAsync()
    {
        return await _context.TiposConta
            .AsNoTracking()
            .OrderBy(t => t.Nome)
            .ToListAsync();
    }
}