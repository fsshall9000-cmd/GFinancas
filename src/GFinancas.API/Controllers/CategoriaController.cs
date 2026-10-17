using GFinancas.Application.DTOs.Categorias;
using GFinancas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFinancas.API.Controllers;

/// <summary>
/// Expõe operações de categorias por meio de endpoints HTTP.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    /// <summary>
    /// Serviço que executa as operações de aplicação de categorias.
    /// </summary>
    private readonly ICategoriaService _categoriaService;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="categoriaService">Serviço que executa as operações de aplicação de categorias.</param>
    public CategoriaController(
        ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    /// <summary>
    /// Cadastra categoria pelo serviço e retorna HTTP 201 com os dados e a localização do recurso.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    [HttpPost("Adicionar")]
    public async Task<ActionResult<CategoriaResponseDto>> Criar(
        CriarCategoriaDto dto)
    {
        // Dados de categoria devolvidos pelo serviço para compor a resposta HTTP.
        var categoria =
            await _categoriaService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = categoria.Id },
            categoria);
    }

    /// <summary>
    /// Consulta categorias pelo serviço e retorna HTTP 200 com a lista.
    /// </summary>
    [HttpGet("Listar")]
    public async Task<
        ActionResult<IEnumerable<CategoriaResponseDto>>> Listar()
    {
        // Registros retornados pelo serviço para compor a lista da resposta HTTP.
        var categorias =
            await _categoriaService.ListarAsync();

        return Ok(categorias);
    }

    /// <summary>
    /// Consulta categoria pelo identificador e retorna o recurso ou HTTP 404 quando o serviço retorna nulo.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    [HttpGet("ObterPorId/{id:int}")]
    public async Task<ActionResult<CategoriaResponseDto>> ObterPorId(
        int id)
    {
        // Dados de categoria devolvidos pelo serviço para compor a resposta HTTP.
        var categoria =
            await _categoriaService.ObterPorIdAsync(id);

        if (categoria is null)
            return NotFound();

        return Ok(categoria);
    }

    /// <summary>Atualiza a categoria e retorna HTTP 200 com os dados atualizados.</summary>
    [HttpPut("Atualizar/{id:int}")]
    public async Task<ActionResult<CategoriaResponseDto>> Atualizar(int id, AtualizarCategoriaDto dto)
    {
        var categoria = await _categoriaService.AtualizarAsync(id, dto);
        return Ok(categoria);
    }

    /// <summary>Remove a categoria e retorna HTTP 204.</summary>
    [HttpDelete("Remover/{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        await _categoriaService.RemoverAsync(id);
        return NoContent();
    }
}