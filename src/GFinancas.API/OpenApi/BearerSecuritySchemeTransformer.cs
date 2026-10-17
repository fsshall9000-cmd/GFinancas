using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace GFinancas.API.OpenApi;

/// <summary>
/// Inclui a autenticação Bearer JWT na descrição OpenAPI quando o esquema está registrado.
/// </summary>
/// <param name="authenticationSchemeProvider">Provedor dos esquemas de autenticação registrados na aplicação.</param>
public sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    /// <summary>
    /// Adiciona o esquema Bearer JWT aos componentes do OpenAPI se ele estiver registrado.
    /// </summary>
    /// <param name="document">Documento OpenAPI que receberá a descrição de segurança.</param>
    /// <param name="context">Contexto fornecido pelo gerador do documento OpenAPI.</param>
    /// <param name="cancellationToken">Sinal de cancelamento da operação assíncrona.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>scheme</c> representa cada esquema de autenticação registrado.
    /// </remarks>
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        // Esquemas de autenticação registrados, consultados para detectar o suporte a Bearer.
        var authenticationSchemes =
            await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.Any(
            scheme => scheme.Name == "Bearer"))
        {
            // Definição do esquema HTTP Bearer JWT que será publicada no OpenAPI.
            var securitySchemes =
                new Dictionary<string, IOpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        In = ParameterLocation.Header,
                        BearerFormat = "JWT"
                    }
                };

            document.Components ??= new OpenApiComponents();

            document.Components.SecuritySchemes =
                securitySchemes;
        }
    }
}