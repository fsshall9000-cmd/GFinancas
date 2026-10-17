using GFinancas.Application.DTOs.Contas;
using GFinancas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFinancas.API.Controllers;

/// <summary>
/// Expõe operações de contas por meio de endpoints HTTP.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContaController : ControllerBase
{
    /// <summary>
    /// Serviço que executa as operações de aplicação de contas.
    /// </summary>
    private readonly IContaService _contaService;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="contaService">Serviço que executa as operações de aplicação de contas.</param>
    public ContaController(IContaService contaService)
    {
        _contaService = contaService;
    }

    /// <summary>
    /// Cadastra conta pelo serviço e retorna HTTP 201 com os dados e a localização do recurso.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    [HttpPost("Adicionar")]
    public async Task<ActionResult<ContaResponseDto>> Criar(
        CriarContaDto dto)
    {
        // Dados de conta devolvidos pelo serviço para compor a resposta HTTP.
        var conta = await _contaService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = conta.Id },
            conta);
    }

    /// <summary>
    /// Consulta contas pelo serviço e retorna HTTP 200 com a lista.
    /// </summary>
    [HttpGet("Listar")]
    public async Task<ActionResult<IEnumerable<ContaResponseDto>>> Listar()
    {
        // Registros retornados pelo serviço para compor a lista da resposta HTTP.
        var contas = await _contaService.ListarAsync();

        return Ok(contas);
    }

    [HttpPut("Atualizar/{id:int}")]
    public async Task<ActionResult<ContaResponseDto>> Atualizar(int id,AtualizarContaDto dto)
    {
        var conta =
            await _contaService.AtualizarAsync(id,dto);

        return Ok(conta);
    }

    [HttpDelete("Remover/{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        await _contaService.RemoverAsync(id);

        return NoContent();
    }



    /// <summary>
    /// Consulta conta pelo identificador e retorna o recurso ou HTTP 404 quando o serviço retorna nulo.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    [HttpGet("BuscarPorId/{id:int}")]
    public async Task<ActionResult<ContaResponseDto>> ObterPorId(
        int id)
    {
        // Dados de conta devolvidos pelo serviço para compor a resposta HTTP.
        var conta = await _contaService.ObterPorIdAsync(id);

        if (conta is null)
            return NotFound();

        return Ok(conta);
    }

    /// <summary>
    /// Retorna o saldo consolidado da conta solicitada.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    [HttpGet("{id:int}/saldo")]
    public async Task<ActionResult<SaldoContaResponseDto>> ObterSaldo(int id)
    {
            // Resumo do saldo da conta retornado pelo serviço para a resposta HTTP.
            var saldo =
            await _contaService.ObterSaldoAsync(id);

            return Ok(saldo);
}
}