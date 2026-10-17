using GFinancas.Application.DTOs.Orcamentos;
using GFinancas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFinancas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrcamentoController : ControllerBase
{
    private readonly IOrcamentoService _orcamentoService;

    public OrcamentoController(
        IOrcamentoService orcamentoService)
    {
        _orcamentoService = orcamentoService;
    }


    // =====================================================
    // CRIAR ORÇAMENTO
    // =====================================================

    [HttpPost("Adicionar")]
    public async Task<ActionResult<OrcamentoResponseDto>> Adicionar(
        [FromBody] CriarOrcamentoDto dto)
    {
        var resultado =
            await _orcamentoService.CriarAsync(dto);

        return Ok(resultado);
    }


    // =====================================================
    // LISTAR ORÇAMENTOS DO MÊS
    // =====================================================

    [HttpGet("Listar")]
    public async Task<ActionResult<IEnumerable<OrcamentoResponseDto>>> Listar(
        [FromQuery] int ano,
        [FromQuery] int mes)
    {
        var resultado =
            await _orcamentoService.ListarPorMesAsync(
                ano,
                mes
            );

        return Ok(resultado);
    }


    // =====================================================
    // BUSCAR POR ID
    // =====================================================

    [HttpGet("ObterPorId/{id:int}")]
    public async Task<ActionResult<OrcamentoResponseDto>> ObterPorId(
        int id)
    {
        var resultado =
            await _orcamentoService.ObterPorIdAsync(id);

        return Ok(resultado);
    }


    // =====================================================
    // ATUALIZAR
    // =====================================================

    [HttpPut("Atualizar/{id:int}")]
    public async Task<ActionResult<OrcamentoResponseDto>> Atualizar(
        int id,
        [FromBody] AtualizarOrcamentoDto dto)
    {
        var resultado =
            await _orcamentoService.AtualizarAsync(
                id,
                dto
            );

        return Ok(resultado);
    }


    // =====================================================
    // REMOVER
    // =====================================================

    [HttpDelete("Remover/{id:int}")]
    public async Task<IActionResult> Remover(
        int id)
    {
        await _orcamentoService.RemoverAsync(id);

        return NoContent();
    }
}