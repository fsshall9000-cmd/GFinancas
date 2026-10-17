using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Infrastructure.Data;

namespace GFinancas.Infrastructure.Repositories;

/// <summary>
/// Executa operações no contexto compartilhado dentro de uma transação de banco de dados.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Recebe e armazena as dependências utilizadas pelo componente.
    /// </summary>
    /// <param name="context">Contexto de banco de dados compartilhado pelos repositórios.</param>
    public UnitOfWork(
        AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Executa a operação e confirma a transação; em caso de falha, reverte a transação e propaga a exceção.
    /// </summary>
    /// <param name="operacao">Operação assíncrona executada dentro da transação.</param>
    public async Task ExecutarEmTransacaoAsync(
        Func<Task> operacao)
    {
        await using var transaction =
            await _context.Database
                .BeginTransactionAsync();

        try
        {
            await operacao();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }
}