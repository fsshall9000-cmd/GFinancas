using GFinancas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFinancas.Infrastructure.Data.Configurations;

/// <summary>
/// Define o mapeamento de tipo de conta, suas restrições e relações no banco de dados.
/// </summary>
public class TipoContaConfiguration : IEntityTypeConfiguration<TipoConta>
{
    /// <summary>
    /// Configura tabela, propriedades e restrições de tipo de conta no modelo do banco.
    /// </summary>
    /// <param name="builder">Construtor usado para configurar o mapeamento da entidade.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>t</c> representa o tipo de conta na expressão de consulta ou mapeamento.
    /// </remarks>
    public void Configure(EntityTypeBuilder<TipoConta> builder)
    {
        builder.ToTable("TiposConta");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(t => t.Nome)
            .IsUnique();

        builder.HasData(
            new TipoConta
            {
                Id = 1,
                Nome = "Conta Corrente"
            },
            new TipoConta
            {
                Id = 2,
                Nome = "Poupança"
            },
            new TipoConta
            {
                Id = 3,
                Nome = "Carteira"
            },
            new TipoConta
            {
                Id = 4,
                Nome = "Conta de Investimento"
            }
        );
    }
}