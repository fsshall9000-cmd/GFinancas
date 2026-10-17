using GFinancas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFinancas.Infrastructure.Data.Configurations;

/// <summary>
/// Configura a tabela de movimentações e os vínculos com a aplicação financeira e a conta.
/// </summary>
public class MovimentacaoAplicacaoConfiguration
    : IEntityTypeConfiguration<MovimentacaoAplicacao>
{
    /// <summary>
    /// Define tabela, chave, campos obrigatórios e relacionamentos com exclusão restrita.
    /// </summary>
    /// <param name="builder">Construtor do mapeamento da entidade no Entity Framework Core.</param>
    public void Configure(
        EntityTypeBuilder<MovimentacaoAplicacao> builder)
    {
        builder.ToTable("MovimentacoesAplicacao");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Valor)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(m => m.Data)
            .IsRequired();

        builder.Property(m => m.Tipo)
            .IsRequired();

        builder.HasOne(m => m.AplicacaoFinanceira)
            .WithMany(a => a.Movimentacoes)
            .HasForeignKey(m => m.AplicacaoFinanceiraId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Conta)
            .WithMany()
            .HasForeignKey(m => m.ContaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}