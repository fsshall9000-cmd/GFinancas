using GFinancas.Application.Interfaces.Repository;
using GFinancas.Domain.Entities;
using GFinancas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Consulta e persiste dados de usuários usando o Entity Framework Core.
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{

    /// <summary>
    /// Contexto do Entity Framework Core usado para consultar e salvar entidades.
    /// </summary>
    private readonly AppDbContext _context;
    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="context">Contexto do Entity Framework Core usado para consultar e salvar entidades.</param>
    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }



    /// <summary>
    /// Adiciona usuário ao contexto e salva as alterações no banco.
    /// </summary>
    /// <param name="usuario">Usuário utilizado na operação.</param>
    public async Task AdicionarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();



    }

    /// <summary>
    /// Busca o usuário pelo e-mail informado e retorna nulo quando não o encontra.
    /// </summary>
    /// <param name="email">E-mail usado para localizar o usuário.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>u</c> representa o usuário na expressão de consulta ou mapeamento.
    /// </remarks>
    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Busca usuário pelo identificador e retorna nulo quando não encontra o registro.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>u</c> representa o usuário na expressão de consulta ou mapeamento.
    /// </remarks>
    public async Task<Usuario?> ObterPorIdAsync(int id)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Lista todos os registros de usuários, ordenados por nome.
    /// </summary>
    /// <remarks>
    /// Nas expressões lambda, <c>u</c> representa o usuário na expressão de consulta ou mapeamento.
    /// </remarks>
    public async Task<IEnumerable<Usuario>> ObterTodosAsync()
    {
        return await _context.Usuarios
        .AsNoTracking()
        .OrderBy(u => u.Nome)
        .ToListAsync();
    }
    public async Task<bool> AtualizarSenhaAsync(int id, string hashAtual, string novoHash)
    {
        var alterados = await _context.Usuarios
            .Where(u => u.Id == id && u.SenhaHash == hashAtual)
            .ExecuteUpdateAsync(updates => updates.SetProperty(u => u.SenhaHash, novoHash));

        return alterados == 1;
    }
}
