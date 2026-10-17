using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GFinancas.Infrastructure.Security;

/// <summary>
/// Emite tokens JWT assinados com os dados do usuário e validade de duas horas.
/// </summary>
public class TokenService : ITokenService
{
    /// <summary>
    /// Configurações da aplicação usadas para acessar os parâmetros de infraestrutura.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="configuration">Configurações da aplicação usadas para acessar os parâmetros de infraestrutura.</param>
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gera um JWT assinado com identificador, nome e e-mail do usuário e validade de duas horas.
    /// </summary>
    /// <param name="usuario">Usuário utilizado na operação.</param>
    public string GerarToken(Usuario usuario)
    {
        // Segredo configurado para assinar o token JWT.
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "Chave JWT não configurada.");

        // Emissor identificado no token JWT.
        var issuer = _configuration["Jwt:Issuer"];
        // Destinatário para o qual o token JWT é emitido.
        var audience = _configuration["Jwt:Audience"];

        // Declarações de identificador, nome e e-mail incorporadas ao token.
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                usuario.Nome),

            new Claim(
                ClaimTypes.Email,
                usuario.Email)
        };

        // Chave simétrica obtida dos bytes UTF-8 do segredo configurado.
        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

        // Credenciais que assinam o token com o algoritmo HMAC-SHA256.
        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        // Objeto JWT com as declarações do usuário, assinatura e prazo de expiração.
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}