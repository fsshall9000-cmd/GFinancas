namespace GFinancas.Application.Interfaces.Repositories;

/// <summary>
/// Define a execução conjunta de operações com confirmação ou reversão da transação.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Executa a operação e confirma a transação; em caso de falha, reverte a transação e propaga a exceção.
    /// </summary>
    /// <param name="operacao">Operação assíncrona executada dentro da transação.</param>
    Task ExecutarEmTransacaoAsync(
        Func<Task> operacao);
}