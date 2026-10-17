using GFinancas.Application.Interfaces.Services;
using GFinancas.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GFinancas.Application;

/// <summary>
/// Centraliza o registro dos serviços de negócio da camada de aplicação.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra os serviços da camada de aplicação com duração por escopo.
    /// </summary>
    /// <param name="services">Coleção em que as dependências serão registradas.</param>
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<
            ITransacaoService,
            TransacaoService>();

        services.AddScoped<
            IUsuarioService,
            UsuarioService>();

        services.AddScoped<IContaService, ContaService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<ITipoContaService, TipoContaService>();
        services.AddScoped<ITransacaoService, TransacaoService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IOrcamentoService, OrcamentoService>();
        services.AddScoped<IAplicacaoFinanceiraService,AplicacaoFinanceiraService>();
        services.AddScoped<IMovimentacaoAplicacaoService,MovimentacaoAplicacaoService>();
        services.AddScoped<ISaldoContaService,SaldoContaService>();


        return services;
    }
}
