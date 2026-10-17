using GFinancas.Application.Exceptions;
using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Application.Interfaces.Repository;
using GFinancas.Application.Interfaces.Services;
using GFinancas.Domain.Enums;

namespace GFinancas.Application.Services;

/// <summary>
/// Calcula o saldo da conta considerando transações e movimentações
/// de aplicações financeiras.
/// </summary>
public class SaldoContaService : ISaldoContaService
{
    private readonly IContaRepository _contaRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IMovimentacaoAplicacaoRepository _movimentacaoRepository;

    public SaldoContaService(
        IContaRepository contaRepository,
        ITransacaoRepository transacaoRepository,
        IMovimentacaoAplicacaoRepository movimentacaoRepository)
    {
        _contaRepository = contaRepository;
        _transacaoRepository = transacaoRepository;
        _movimentacaoRepository = movimentacaoRepository;
    }

    /// <summary>
    /// Calcula o saldo atual da conta.
    /// </summary>
    public async Task<decimal> CalcularAsync(int contaId)
    {
        var conta =
            await _contaRepository.ObterPorIdAsync(contaId);

        if (conta is null)
        {
            throw new NaoEncontradoException(
                "Conta não encontrada.");
        }

        var transacoes =
            await _transacaoRepository
                .ObterPorContaIdAsync(contaId);

        var totalReceitas = transacoes
            .Where(t => t.Tipo == TipoTransacao.Receita)
            .Sum(t => t.Valor);

        var totalDespesas = transacoes
            .Where(t => t.Tipo == TipoTransacao.Despesa)
            .Sum(t => t.Valor);

        var totalAportes =
            await _movimentacaoRepository
                .ObterTotalAportesPorContaAsync(contaId);

        var totalResgates =
            await _movimentacaoRepository
                .ObterTotalResgatesPorContaAsync(contaId);

        return conta.SaldoInicial
               + totalReceitas
               - totalDespesas
               - totalAportes
               + totalResgates;
    }

    /// <summary>
    /// Calcula o saldo da conta considerando apenas operações
    /// realizadas até a data informada.
    /// </summary>
   public async Task<decimal> CalcularAteDataAsync(
    int contaId,
    DateTime dataLimite)
{
    var conta = await _contaRepository.ObterPorIdAsync(contaId);

    if (conta is null)
        throw new NaoEncontradoException(
            "Conta não encontrada.");

    /*
     * Convertemos a data consultada para DateOnly porque
     * DataCriacao da conta também é DateOnly.
     */
    var dataConsulta =
        DateOnly.FromDateTime(dataLimite);

    /*
     * A conta ainda não fazia parte do patrimônio
     * antes da sua data de criação.
     */
    if (dataConsulta < conta.DataCriacao)
        return 0m;

    var totalReceitas =
        await _transacaoRepository
            .ObterTotalReceitasAteDataAsync(
                contaId,
                dataLimite);

    var totalDespesas =
        await _transacaoRepository
            .ObterTotalDespesasAteDataAsync(
                contaId,
                dataLimite);

    var totalAportes =
        await _movimentacaoRepository
            .ObterTotalAportesPorContaAteDataAsync(
                contaId,
                dataLimite);

    var totalResgates =
        await _movimentacaoRepository
            .ObterTotalResgatesPorContaAteDataAsync(
                contaId,
                dataLimite);

    return conta.SaldoInicial
           + totalReceitas
           - totalDespesas
           - totalAportes
           + totalResgates;
}
}