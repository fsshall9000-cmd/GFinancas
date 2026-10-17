using GFinancas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GFinancas.Infrastructure.Data;

/// <summary>
/// Gerencia o acesso às entidades financeiras e seus mapeamentos no Entity Framework Core.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Inicializa o contexto com as opções de acesso ao banco.
    /// </summary>
    /// <param name="options">Opções de configuração do contexto de banco de dados.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Conjunto de usuários disponível para consultas e persistência no banco.
    /// </summary>
    public DbSet<Usuario> Usuarios { get; set; }

    /// <summary>
    /// Conjunto de contas disponível para consultas e persistência no banco.
    /// </summary>
    public DbSet<Conta> Contas { get; set; }

    /// <summary>
    /// Conjunto de tipos de conta disponível para consultas e persistência no banco.
    /// </summary>
    public DbSet<TipoConta> TiposConta { get; set; }

    /// <summary>
    /// Conjunto de categorias disponível para consultas e persistência no banco.
    /// </summary>
    public DbSet<Categoria> Categorias { get; set; }

    /// <summary>
    /// Historico de valores das aplicações financeiras disponível para consultas e persistência no banco.
    /// </summary>
    public DbSet<HistoricoValorAplicacao> HistoricosValoresAplicacoes
    { get; set; }


    public DbSet<Orcamento> Orcamentos { get; set; }

    /// <summary>
    /// Conjunto de transações disponível para consultas e persistência no banco.
    /// </summary>
    public DbSet<Transacao> Transacoes { get; set; }

    public DbSet<AplicacaoFinanceira> AplicacoesFinanceiras { get; set; }

    public DbSet<MovimentacaoAplicacao> MovimentacoesAplicacao { get; set; }

    /// <summary>
    /// Aplica as configurações de entidades encontradas no assembly do contexto.
    /// </summary>
    /// <param name="modelBuilder">Construtor do modelo de entidades do Entity Framework Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}