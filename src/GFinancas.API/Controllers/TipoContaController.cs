using GFinancas.Application.DTOs.TiposConta;
using GFinancas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFinancas.API.Controllers;

/// <summary>
/// Expõe operações relacionadas aos tipos de conta.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TipoContaController : ControllerBase
{
    private readonly ITipoContaService _tipoContaService;

    public TipoContaController(
        ITipoContaService tipoContaService)
    {
        _tipoContaService = tipoContaService;
    }

    /// <summary>
    /// Retorna os tipos de conta disponíveis.
    /// </summary>
    [HttpGet("Listar")]
    public async Task<ActionResult<
        IEnumerable<TipoContaResponseDto>>> Listar()
    {
        var tiposConta =
            await _tipoContaService.ListarAsync();

        return Ok(tiposConta);
    }
}