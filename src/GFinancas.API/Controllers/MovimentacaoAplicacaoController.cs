using GFinancas.Application.DTOs.Aplicacoes;
using GFinancas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFinancas.API.Controllers;

/// <summary>
/// Expõe os endpoints autenticados de registro e consulta de aportes e resgates.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MovimentacaoAplicacaoController
    : ControllerBase
{
    private readonly IMovimentacaoAplicacaoService
        _movimentacaoService;

    /// <summary>
    /// Recebe e armazena as dependências utilizadas pelo componente.
    /// </summary>
    /// <param name="movimentacaoService">Serviço responsável pelo registro e consulta de movimentações.</param>
    public MovimentacaoAplicacaoController(
        IMovimentacaoAplicacaoService movimentacaoService)
    {
        _movimentacaoService =
            movimentacaoService;
    }

    // POST: api/MovimentacaoAplicacao/Registrar
    /// <summary>
    /// Registra um aporte ou resgate e retorna os dados da movimentação.
    /// </summary>
    /// <param name="dto">Dados recebidos para a operação.</param>
    [HttpPost("Registrar")]
    public async Task<
        ActionResult<MovimentacaoAplicacaoResponseDto>>
        Registrar(
            [FromBody] CriarMovimentacaoAplicacaoDto dto)
    {
        var resultado =
            await _movimentacaoService
                .RegistrarAsync(dto);

        return Ok(resultado);
    }

    // GET:
    // api/MovimentacaoAplicacao/ListarPorAplicacao/1
    /// <summary>
    /// Retorna o histórico de movimentações da aplicação do usuário autenticado.
    /// </summary>
    /// <param name="aplicacaoId">Identificador da aplicação financeira.</param>
    [HttpGet("ListarPorAplicacao/{aplicacaoId:int}")]
    public async Task<ActionResult<
        IEnumerable<MovimentacaoAplicacaoResponseDto>>>
        ListarPorAplicacao(
            int aplicacaoId)
    {
        var resultado =
            await _movimentacaoService
                .ListarPorAplicacaoAsync(
                    aplicacaoId);

        return Ok(resultado);
    }
}