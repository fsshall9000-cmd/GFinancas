using GFinancas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFinancas.Infrastructure.Data.Configurations;

/// <summary>
/// Configura a tabela, as propriedades e o vínculo da aplicação financeira com o usuário.
/// </summary>
public class AplicacaoFinanceiraConfiguration
    : IEntityTypeConfiguration<AplicacaoFinanceira>
{
    /// <summary>
    /// Define tabela, chave, campos obrigatórios e relacionamentos com exclusão restrita.
    /// </summary>
    /// <param name="builder">Construtor do mapeamento da entidade no Entity Framework Core.</param>
    public void Configure(
        EntityTypeBuilder<AplicacaoFinanceira> builder)
    {
        builder.ToTable("AplicacoesFinanceiras");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.Tipo)
            .IsRequired();

        builder.Property(a => a.Instituicao)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.DataInicio)
            .IsRequired();

        builder.Property(a => a.ValorAtual)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(a => a.Ativa)
            .IsRequired();

        builder.HasOne(a => a.Usuario)
            .WithMany()
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

       
    }
}