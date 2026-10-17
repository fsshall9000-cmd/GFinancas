namespace GFinancas.Application.Interfaces.Services;

/// <summary>
/// Define o cálculo do saldo da conta, incluindo aportes e resgates.
/// </summary>
public interface ISaldoContaService
{
    /// <summary>
    /// Soma saldo inicial, receitas e resgates e subtrai despesas e aportes; rejeita contas inexistentes.
    /// </summary>
    /// <param name="contaId">Identificador da conta financeira.</param>
    Task<decimal> CalcularAsync(int contaId);


    /// <summary>
    /// Soma saldo inicial, receitas e resgates e subtrai despesas e aportes até a data limite; rejeita contas inexistentes.
    /// </summary>
    /// <param name="contaId"></param>
    /// <param name="dataLimite"></param>
    /// <returns></returns>
    Task<decimal> CalcularAteDataAsync(int contaId,DateTime dataLimite);
}