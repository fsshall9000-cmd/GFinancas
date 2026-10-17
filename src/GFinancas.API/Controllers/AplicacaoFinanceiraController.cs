using GFinancas.Application.DTOs.Aplicacoes;
using GFinancas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFinancas.API.Controllers;

/// <summary>
/// Expõe os endpoints autenticados de cadastro, consulta, atualização e desativação de aplicações financeiras.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AplicacaoFinanceiraController : ControllerBase
{
    private readonly IAplicacaoFinanceiraService
        _aplicacaoService;

    /// <summary>
    /// Recebe e armazena as dependências utilizadas pelo componente.
    /// </summary>
    /// <param name="aplicacaoService">Serviço responsável pelas operações de aplicações financeiras.</param>
    public AplicacaoFinanceiraController(
        IAplicacaoFinanceiraService aplicacaoService)
    {
        _aplicacaoService = aplicacaoService;
    }

    // POST: api/AplicacaoFinanceira/Adicionar
    /// <summary>
    /// Cadastra a aplicação e retorna HTTP 201 com o endereço para consulta do registro.
    /// </summary>
    /// <param name="dto">Dados recebidos para a operação.</param>
    [HttpPost("Adicionar")]
    public async Task<ActionResult<AplicacaoFinanceiraResponseDto>>
        Adicionar(
            [FromBody] CriarAplicacaoFinanceiraDto dto)
    {
        var resultado =
            await _aplicacaoService
                .CriarAsync(dto);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = resultado.Id },
            resultado);
    }

    // GET: api/AplicacaoFinanceira/Listar
    /// <summary>
    /// Retorna as aplicações financeiras do usuário autenticado.
    /// </summary>
    [HttpGet("Listar")]
    public async Task<ActionResult<
        IEnumerable<AplicacaoFinanceiraResponseDto>>>
        Listar()
    {
        var resultado =
            await _aplicacaoService
                .ListarAsync();

        return Ok(resultado);
    }

    // GET: api/AplicacaoFinanceira/ObterPorId/1
    /// <summary>
    /// Retorna a aplicação financeira do usuário autenticado pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da aplicação financeira.</param>
    [HttpGet("ObterPorId/{id:int}")]
    public async Task<ActionResult<AplicacaoFinanceiraResponseDto>>
        ObterPorId(int id)
    {
        var resultado =
            await _aplicacaoService
                .ObterPorIdAsync(id);

        return Ok(resultado);
    }

    // PUT: api/AplicacaoFinanceira/Atualizar/1
    /// <summary>
    /// Atualiza a aplicação financeira e retorna os dados resultantes.
    /// </summary>
    /// <param name="id">Identificador da aplicação financeira.</param>
    /// <param name="dto">Dados recebidos para a operação.</param>
    [HttpPut("Atualizar/{id:int}")]
    public async Task<ActionResult<AplicacaoFinanceiraResponseDto>>
        Atualizar(
            int id,
            [FromBody] AtualizarAplicacaoFinanceiraDto dto)
    {
        var resultado =
            await _aplicacaoService
                .AtualizarAsync(
                    id,
                    dto);

        return Ok(resultado);
    }

    [HttpPatch("AtualizarValor/{id:int}")]
    public async Task<ActionResult<AplicacaoFinanceiraResponseDto>>
        AtualizarValor(
            int id,
            [FromBody] AtualizarValorAplicacaoDto dto)
    {
        var resultado =
            await _aplicacaoService
                .AtualizarValorAsync(id, dto);

        return Ok(resultado);
    }

    [HttpGet("{id:int}/HistoricoValores")]
    public async Task<ActionResult<IEnumerable<HistoricoValorAplicacaoResponseDto>>>
        ListarHistoricoValores(int id)
    {
        var resultado =
            await _aplicacaoService
                .ListarHistoricoValoresAsync(id);

        return Ok(resultado);
    }

    // PATCH: api/AplicacaoFinanceira/Desativar/1
    /// <summary>
    /// Desativa a aplicação financeira e retorna HTTP 204.
    /// </summary>
    /// <param name="id">Identificador da aplicação financeira.</param>
    [HttpPatch("Desativar/{id:int}")]
    public async Task<IActionResult>
        Desativar(int id)
    {
        await _aplicacaoService
            .DesativarAsync(id);

        return NoContent();
    }
}
