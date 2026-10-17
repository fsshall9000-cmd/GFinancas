using System.Security.Claims;
using GFinancas.Application.Exceptions;
using GFinancas.Application.Interfaces.Security;
using Microsoft.AspNetCore.Http;

namespace GFinancas.API.Services;

/// <summary>
/// Obtém o identificador do usuário a partir das declarações da requisição HTTP atual.
/// </summary>
public class UsuarioAtualService : IUsuarioAtualService
{
    /// <summary>
    /// Acesso à requisição HTTP atual e às declarações do usuário.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="httpContextAccessor">Acesso à requisição HTTP atual e às declarações do usuário.</param>
    public UsuarioAtualService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Identificador do usuário obtido da requisição atual; a implementação rejeita declarações ausentes ou inválidas.
    /// </summary>
    public int UsuarioId
    {
        get
        {
            // Valor textual da declaração NameIdentifier presente na requisição atual.
            var usuarioIdClaim = _httpContextAccessor
                .HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
            {
                throw new ValidacaoException(
                    "Usuário não autenticado.");
            }

            // usuarioId recebe o identificador numérico extraído da declaração de autenticação.
            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
            {
                throw new ValidacaoException(
                    "Identificador do usuário inválido.");
            }

            return usuarioId;
        }
    }
}