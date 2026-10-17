using GFinancas.Application.DTOs.Dashboard;

namespace GFinancas.Application.Interfaces.Services;

/// <summary>
/// Define as consultas de resumo, despesas por categoria e histórico mensal.
/// </summary>
public interface IDashboardService
{
  /// <summary>
  /// Soma receitas e despesas do usuário atual no mês e calcula o resultado financeiro.
  /// </summary>
  /// <param name="ano">Ano de referência da consulta.</param>
  /// <param name="mes">Mês de referência da consulta, de 1 a 12.</param>
  Task<ResumoFinanceiroDto> ObterResumoAsync(
    int ano,
    int mes);

/// <summary>
/// Agrupa as despesas mensais do usuário atual por categoria e calcula sua participação percentual.
/// </summary>
/// <param name="ano">Ano de referência da consulta.</param>
/// <param name="mes">Mês de referência da consulta, de 1 a 12.</param>
Task<IEnumerable<DespesaPorCategoriaDto>>
    ObterDespesasPorCategoriaAsync(
        int ano,
        int mes);

/// <summary>
/// Produz os totais dos doze meses do ano para o usuário atual, incluindo meses sem movimentação.
/// </summary>
/// <param name="ano">Ano de referência da consulta.</param>
Task<IEnumerable<HistoricoMensalDto>>
    ObterHistoricoMensalAsync(
        int ano);

Task<ResumoInvestimentosDto>
    ObterResumoInvestimentosAsync();

 Task<ResumoPatrimonioDto>
        ObterPatrimonioAsync();

Task<IEnumerable<EvolucaoPatrimonialDto>>
    ObterEvolucaoPatrimonialAsync(int ano);
    
}