using GFinancas.Application.Dto.Transacoes;
using GFinancas.Application.DTOs.Transacoes;
using GFinancas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GFinancas.Application.DTOs.Comum;

namespace GFinancas.API.Controllers;

/// <summary>
/// Expõe operações de transações por meio de endpoints HTTP.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransacaoController : ControllerBase
{
    /// <summary>
    /// Serviço que executa as operações de aplicação de transações.
    /// </summary>
    private readonly ITransacaoService _transacaoService;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="transacaoService">Serviço que executa as operações de aplicação de transações.</param>
    public TransacaoController(ITransacaoService transacaoService)
    {
        _transacaoService = transacaoService;
    }

    /// <summary>
    /// Cadastra transação pelo serviço e retorna HTTP 201 com os dados e a localização do recurso.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    [HttpPost("Adicionar")]
    public async Task<ActionResult<TransacaoResponseDto>> Criar(
        CriarTransacaoDto dto)
    {
        // Dados de transação devolvidos pelo serviço para compor a resposta HTTP.
        var transacao = await _transacaoService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = transacao.Id },
            transacao);
    }

    /// <summary>
    /// Consulta transações pelo serviço e retorna HTTP 200 com a lista.
    /// </summary>
    [HttpGet("Listar")]
    public async Task<ActionResult<IEnumerable<TransacaoResponseDto>>> Listar()
    {
        // Registros retornados pelo serviço para compor a lista da resposta HTTP.
        var transacoes = await _transacaoService.ListarAsync();

        return Ok(transacoes);
    }

    /// <summary>
    /// Consulta transação pelo identificador e retorna o recurso ou HTTP 404 quando o serviço retorna nulo.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    [HttpGet("BuscarPorId/{id:int}")]
    public async Task<ActionResult<TransacaoResponseDto>> ObterPorId(int id)
    {
        // Dados de transação devolvidos pelo serviço para compor a resposta HTTP.
        var transacao = await _transacaoService.ObterPorIdAsync(id);

        if (transacao is null)
            return NotFound();

        return Ok(transacao);
    }

    /// <summary>
    /// Encaminha os filtros da query string ao serviço e retorna as transações encontradas.
    /// </summary>
    /// <param name="filtro">Usuário e critérios de seleção das transações.</param>
    [HttpGet("filtro")]
    public async Task<ActionResult<IEnumerable<TransacaoResponseDto>>> Filtrar([FromQuery] FiltroTransacaoDto filtro)
    {
        // Registros retornados pelo serviço para compor a lista da resposta HTTP.
        var transacoes = await _transacaoService.FiltrarAsync(filtro);

        return Ok(transacoes);
    }

    /// <summary>
    /// Atualiza a transação indicada e retorna HTTP 200 com os novos dados.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    [HttpPut("Atualizar/{id:int}")]
    public async Task<ActionResult<TransacaoResponseDto>> Atualizar(
    int id,
    AtualizarTransacaoDto dto)
    {
        // Dados de transação devolvidos pelo serviço para compor a resposta HTTP.
        var transacao =
        await _transacaoService.AtualizarAsync(id, dto);

        return Ok(transacao);
    }
    
    /// <summary>
    /// Solicita a exclusão da transação e retorna HTTP 204 sem corpo.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    [HttpDelete("Remover/{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        await _transacaoService.RemoverAsync(id);

        return NoContent();
    }

   
    /// <summary>
    /// Retorna a página de transações e seus metadados, usando os parâmetros da query string.
    /// </summary>
    /// <param name="pagina">Número da página solicitada, começando em 1.</param>
    /// <param name="tamanhoPagina">Quantidade de itens solicitada por página.</param>
    [HttpGet("paginado")]
    public async Task<IActionResult> ListarPaginado(
    [FromQuery] int pagina = 1,
    [FromQuery] int tamanhoPagina = 10)
{
    // Página de transações e metadados retornados pelo serviço.
    var resultado = await _transacaoService
        .ListarPaginadoAsync(
            pagina,
            tamanhoPagina);

    return Ok(resultado);
}
}