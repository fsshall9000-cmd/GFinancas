using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Domain.Entities;
using GFinancas.Domain.Enums;
using GFinancas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GFinancas.Infrastructure.Repositories;

/// <summary>
/// Consulta e persiste dados de categorias usando o Entity Framework Core.
/// </summary>
public class CategoriaRepository : ICategoriaRepository
{
    /// <summary>
    /// Contexto do Entity Framework Core usado para consultar e salvar entidades.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="context">Contexto do Entity Framework Core usado para consultar e salvar entidades.</param>
    public CategoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona categoria ao contexto e salva as alterações no banco.
    /// </summary>
    /// <param name="categoria">Categoria utilizada na operação.</param>
    public async Task AdicionarAsync(Categoria categoria)
    {
        await _context.Categorias.AddAsync(categoria);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Lista categorias do usuário informado, em ordem de nome.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário cujos dados serão consultados.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>c</c> representa a categoria na expressão de consulta ou mapeamento.
    /// </remarks>
    public async Task<IEnumerable<Categoria>> ObterPorUsuarioIdAsync(
    int usuarioId)
{
    return await _context.Categorias
        .AsNoTracking()
        .Where(c => c.UsuarioId == usuarioId)
        .OrderBy(c => c.Nome)
        .ToListAsync();
}
    /// <summary>
    /// Busca categoria pelo identificador e retorna nulo quando não encontra o registro.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>c</c> representa a categoria na expressão de consulta ou mapeamento.
    /// </remarks>
    public async Task<Categoria?> ObterPorIdAsync(int id)
    {
        return await _context.Categorias
            .AsNoTracking()
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Lista todos os registros de categorias, ordenados por nome.
    /// </summary>
    /// <remarks>
    /// Nas expressões lambda, <c>c</c> representa a categoria na expressão de consulta ou mapeamento.
    /// </remarks>
    public async Task<IEnumerable<Categoria>> ObterTodasAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .Include(c => c.Usuario)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    /// <summary>
    /// Busca uma categoria pela combinação de nome, tipo e proprietário.
    /// </summary>
    /// <param name="nome">Nome da categoria procurada.</param>
    /// <param name="tipo">Tipo de movimentação: receita ou despesa.</param>
    /// <param name="usuarioId">Identificador do usuário cujos dados serão consultados.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>c</c> representa a categoria na expressão de consulta ou mapeamento.
    /// </remarks>
    public async Task<Categoria?> ObterPorNomeTipoEUsuarioAsync(
        string nome,
        TipoTransacao tipo,
        int usuarioId)
    {
        return await _context.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.Nome == nome &&
                c.Tipo == tipo &&
                c.UsuarioId == usuarioId);
    }

    /// <summary>Atualiza somente a categoria e salva as alterações.</summary>
    public async Task AtualizarAsync(Categoria categoria)
    {
        _context.Entry(categoria).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    /// <summary>Remove a categoria e salva as alterações.</summary>
    public async Task RemoverAsync(Categoria categoria)
    {
        _context.Entry(categoria).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
    }

    /// <summary>Verifica se existem transações vinculadas à categoria.</summary>
    public Task<bool> PossuiTransacoesAsync(int categoriaId)
    {
        return _context.Transacoes.AnyAsync(t => t.CategoriaId == categoriaId);
    }
}