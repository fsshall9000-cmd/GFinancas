using GFinancas.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GFinancas.API.ExceptionHandlers;

/// <summary>
/// Converte exceções da aplicação em respostas HTTP no formato Problem Details.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Seleciona o status HTTP da exceção, escreve Problem Details e indica que o erro foi tratado.
    /// </summary>
    /// <param name="httpContext">Contexto HTTP cuja resposta receberá os detalhes do erro.</param>
    /// <param name="exception">Exceção a converter em resposta HTTP.</param>
    /// <param name="cancellationToken">Sinal de cancelamento da operação assíncrona.</param>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // statusCode e title definem o código HTTP e o título correspondentes ao tipo da exceção.
        var (statusCode, title) = exception switch
        {
            ValidacaoException =>
                (StatusCodes.Status400BadRequest, "Erro de validação"),

            NaoAutorizadoException =>
                (StatusCodes.Status401Unauthorized, "Não autorizado"),

            NaoEncontradoException =>
                (StatusCodes.Status404NotFound, "Recurso não encontrado"),

            ConflitoException =>
                (StatusCodes.Status409Conflict, "Conflito"),

            _ =>
                (StatusCodes.Status500InternalServerError, "Erro interno")
        };

        // Corpo padronizado do erro, com status HTTP, título e mensagem da exceção.
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}