using GFinancas.Application.DTOs.Dashboard;
using GFinancas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFinancas.API.Controllers;

/// <summary>
/// Expõe os indicadores financeiros do usuário autenticado por HTTP.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    /// <summary>
    /// Serviço responsável pelos indicadores e históricos financeiros.
    /// </summary>
    private readonly IDashboardService _dashboardService;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="dashboardService">Serviço responsável pelos indicadores e históricos financeiros.</param>
    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Retorna o resumo financeiro do ano e mês informados.
    /// </summary>
    /// <param name="ano">Ano de referência da consulta.</param>
    /// <param name="mes">Mês de referência da consulta, de 1 a 12.</param>
    [HttpGet("resumo")]
    public async Task<IActionResult> ObterResumo(
        int ano,
        int mes)
    {
        // Indicadores financeiros calculados para o mês solicitado.
        var resultado = await _dashboardService
        .ObterResumoAsync(ano, mes);

    return Ok(resultado);
    }

    [HttpGet("Patrimonio")]
    public async Task<ActionResult<ResumoPatrimonioDto>>
    ObterPatrimonio()
    {
        var resultado =
        await _dashboardService
            .ObterPatrimonioAsync();

        return Ok(resultado);
    }

   /// <summary>
   /// Retorna a distribuição das despesas por categoria no ano e mês informados.
   /// </summary>
   /// <param name="ano">Ano de referência da consulta.</param>
   /// <param name="mes">Mês de referência da consulta, de 1 a 12.</param>
   [HttpGet("despesas-por-categoria")]
    public async Task<IActionResult> ObterDespesasPorCategoria(
    int ano,
    int mes)
{
    // Totais e percentuais das despesas agrupadas por categoria.
    var resultado = await _dashboardService
        .ObterDespesasPorCategoriaAsync(ano, mes);

    return Ok(resultado);
}
    /// <summary>
    /// Retorna o histórico financeiro mensal do ano informado.
    /// </summary>
    /// <param name="ano">Ano de referência da consulta.</param>
    [HttpGet("historico")]
public async Task<IActionResult> ObterHistorico(
    int ano)
{
    // Indicadores de cada mês do ano solicitado.
    var resultado = await _dashboardService
        .ObterHistoricoMensalAsync(ano);

    return Ok(resultado);
}

[HttpGet("Investimentos")]
public async Task<ActionResult<ResumoInvestimentosDto>>
    ObterResumoInvestimentos()
{
    var resultado =
        await _dashboardService
            .ObterResumoInvestimentosAsync();

    return Ok(resultado);
}

/// <summary>
/// Retorna a evolução patrimonial mensal do usuário autenticado.
/// </summary>
[HttpGet("EvolucaoPatrimonial")]
public async Task<ActionResult<IEnumerable<EvolucaoPatrimonialDto>>>
    ObterEvolucaoPatrimonial([FromQuery] int ano)
{
    var resultado =
        await _dashboardService
            .ObterEvolucaoPatrimonialAsync(ano);

    return Ok(resultado);
}


}