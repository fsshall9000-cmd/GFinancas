using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Application.Interfaces.Repository;
using GFinancas.Infrastructure.Data;
using GFinancas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Infrastructure.Security;

namespace GFinancas.Infrastructure;

/// <summary>
/// Centraliza o registro dos serviços de infraestrutura e de acesso ao banco.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configura o contexto MySQL e registra os repositórios e serviços de segurança.
    /// </summary>
    /// <param name="services">Coleção em que as dependências serão registradas.</param>
    /// <param name="configuration">Configurações da aplicação usadas para acessar os parâmetros de infraestrutura.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>options</c> configura o contexto para utilizar o provedor MySQL.
    /// </remarks>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // String de conexão DefaultConnection usada pelo provedor MySQL.
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' não encontrada.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySQL(connectionString));

        services.AddScoped<
            ITransacaoRepository,
            TransacaoRepository>();

        services.AddScoped<
            IUsuarioRepository,
            UsuarioRepository>();

        services.AddScoped<IContaRepository, ContaRepository>();

        services.AddScoped<ITipoContaRepository, TipoContaRepository>();

        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IOrcamentoRepository, OrcamentoRepository>();
        
        services.AddScoped<ISenhaHasher, SenhaHasher>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IAplicacaoFinanceiraRepository,AplicacaoFinanceiraRepository>();
        services.AddScoped<IMovimentacaoAplicacaoRepository,MovimentacaoAplicacaoRepository>();

        services.AddScoped<IHistoricoValorAplicacaoRepository, HistoricoValorAplicacaoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
