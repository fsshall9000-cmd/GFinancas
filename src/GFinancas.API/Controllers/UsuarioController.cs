using GFinancas.Application.Dto.Usuarios;
using GFinancas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Expõe operações de usuários por meio de endpoints HTTP.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    /// <summary>
    /// Serviço que executa as operações de aplicação de usuários.
    /// </summary>
    private IUsuarioService _usuarioService;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="usuarioService">Serviço que executa as operações de aplicação de usuários.</param>
    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }   
    
    /// <summary>
    /// Retorna os dados cadastrais do usuário autenticado.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> ObterUsuarioAtual()
    {
        // Dados de usuário devolvidos pelo serviço para compor a resposta HTTP.
        var usuario = await _usuarioService.ObterUsuarioAtualAsync();

        return Ok(usuario);
    }



    /// <summary>
    /// Cadastra usuário pelo serviço e retorna HTTP 201 com os dados e a localização do recurso.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    [AllowAnonymous]
    [HttpPost("Adicionar")]
    public async Task<ActionResult<UsuarioResponseDto>> Criar(CriarUsuarioDto dto)
    {
        // Dados de usuário devolvidos pelo serviço para compor a resposta HTTP.
        var usuario = await _usuarioService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = usuario.Id },
            usuario);
    }

    /// <summary>
    /// Consulta usuários pelo serviço e retorna HTTP 200 com a lista.
    /// </summary>
    [HttpGet("Listar")]
    public async Task<
    ActionResult<IEnumerable<UsuarioResponseDto>>> Listar()
    {
        // Registros retornados pelo serviço para compor a lista da resposta HTTP.
        var usuarios =
            await _usuarioService.ListarAsync();

        return Ok(usuarios);
    }

    /// <summary>
    /// Consulta usuário pelo identificador e retorna o recurso ou HTTP 404 quando o serviço retorna nulo.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    [Authorize]
    [HttpGet("BuscarPorId/{id:int}")]
    public async Task<ActionResult<UsuarioResponseDto>> ObterPorId(
        int id)
    {
        // Dados de usuário devolvidos pelo serviço para compor a resposta HTTP.
        var usuario =
            await _usuarioService.ObterPorIdAsync(id);

        if (usuario is null)
            return NotFound();

        return Ok(usuario);
    }




}
