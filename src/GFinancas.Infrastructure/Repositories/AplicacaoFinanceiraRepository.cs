using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Domain.Entities;
using GFinancas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GFinancas.Infrastructure.Repositories;

/// <summary>
/// Persiste aplicações financeiras e consulta os registros pertencentes ao usuário informado.
/// </summary>
public class AplicacaoFinanceiraRepository
    : IAplicacaoFinanceiraRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Recebe e armazena as dependências utilizadas pelo componente.
    /// </summary>
    /// <param name="context">Contexto de banco de dados compartilhado pelos repositórios.</param>
    public AplicacaoFinanceiraRepository(
        AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona o registro ao contexto e salva as alterações no banco de dados.
    /// </summary>
    /// <param name="aplicacao">Aplicação financeira utilizada na operação.</param>
    public async Task<AplicacaoFinanceira> AdicionarAsync(
        AplicacaoFinanceira aplicacao)
    {
        await _context.AplicacoesFinanceiras
            .AddAsync(aplicacao);

        await _context.SaveChangesAsync();

        return aplicacao;
    }

    /// <summary>
    /// Busca a aplicação pelo identificador, restringindo a consulta ao usuário.
    /// </summary>
    /// <param name="id">Identificador da aplicação financeira.</param>
    /// <param name="usuarioId">Identificador do usuário proprietário dos registros.</param>
    public async Task<AplicacaoFinanceira?> ObterPorIdAsync(
        int id,
        int usuarioId)
    {
        return await _context.AplicacoesFinanceiras
            .AsNoTracking()
            .FirstOrDefaultAsync(a =>
                a.Id == id &&
                a.UsuarioId == usuarioId);
    }

    /// <summary>
    /// Lista as aplicações financeiras do usuário.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário proprietário dos registros.</param>
    public async Task<IEnumerable<AplicacaoFinanceira>> ListarAsync(
        int usuarioId)
    {
        return await _context.AplicacoesFinanceiras
            .AsNoTracking()
            .Where(a =>
                a.UsuarioId == usuarioId)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    /// <summary>
    /// Atualiza os dados da aplicação financeira.
    /// </summary>
    /// <param name="aplicacao">Aplicação financeira utilizada na operação.</param>
    public async Task AtualizarAsync(
        AplicacaoFinanceira aplicacao)
    {
        _context.AplicacoesFinanceiras
            .Update(aplicacao);

        await _context.SaveChangesAsync();
    }
}