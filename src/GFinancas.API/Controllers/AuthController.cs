using GFinancas.Application.DTOs.Auth;
using GFinancas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GFinancas.API.Controllers;


/// <summary>
/// Expõe o endpoint HTTP de autenticação por e-mail e senha.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Serviço responsável pela autenticação das credenciais.
    /// </summary>
    private readonly IAuthService _authService;
    private readonly IRecuperacaoSenhaService _recuperacaoSenha;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="authService">Serviço responsável pela autenticação das credenciais.</param>
    public AuthController(IAuthService authService, IRecuperacaoSenhaService recuperacaoSenha)
    {
        _authService = authService;
        _recuperacaoSenha = recuperacaoSenha;
    }

    /// <summary>
    /// Autentica as credenciais e retorna HTTP 200 com o token e os dados do usuário.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(
        LoginDto dto)
    {
        // Token e dados do usuário retornados pela autenticação.
        var resultado =
            await _authService.LoginAsync(dto);

        return Ok(resultado);
    }
    [AllowAnonymous]
    [EnableRateLimiting("recuperacao-senha")]
    [HttpPost("esqueceu-senha")]
    public async Task<IActionResult> EsqueceuSenha(RecuperarSenhaDto dto)
    {
        await _recuperacaoSenha.SolicitarAsync(dto);
        return Ok(new { mensagem = "Se o e-mail estiver cadastrado, você receberá um link para redefinir sua senha." });
    }

    [AllowAnonymous]
    [EnableRateLimiting("recuperacao-senha")]
    [HttpPost("redefinir-senha")]
    public async Task<IActionResult> RedefinirSenha(RedefinirSenhaDto dto)
    {
        await _recuperacaoSenha.RedefinirAsync(dto);
        return Ok(new { mensagem = "Senha atualizada. Entre com sua nova senha." });
    }
}