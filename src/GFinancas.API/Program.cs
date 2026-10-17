using GFinancas.Application;
using GFinancas.Infrastructure;
using GFinancas.API.ExceptionHandlers;
using GFinancas.API.Services;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Application.Interfaces.Services;
using Scalar.AspNetCore;
using GFinancas.API.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

// Monta a aplicação web e reúne os serviços e as configurações de inicialização.
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddDataProtection();
builder.Services.AddScoped<IRecuperacaoSenhaService, RecuperacaoSenhaService>();
builder.Services.AddScoped<IEmailRecuperacaoSenha, EmailRecuperacaoSenha>();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("recuperacao-senha", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "local",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy => policy
        .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:4200"])
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// Chave configurada para validar a assinatura dos tokens JWT recebidos.
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "A chave JWT não foi configurada.");

// Emissor esperado dos tokens JWT recebidos.
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

// Destinatário esperado dos tokens JWT recebidos.
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    // options reúne os parâmetros de validação dos tokens JWT.
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    IUsuarioAtualService,
    UsuarioAtualService>();

// OpenAPI
//builder.Services.AddOpenApi();

// options configura a geração do documento OpenAPI e seu esquema de segurança.
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<
        BearerSecuritySchemeTransformer>();
});


// Clean Architecture
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();

// Aplicação web construída, na qual são configurados os middlewares e endpoints.
var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // Gera o documento OpenAPI
    app.MapOpenApi();

    // Interface visual Scalar
    app.MapScalarApiReference();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("Frontend");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();