using GFinancas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GFinancas.Infrastructure.Data.Configurations;

/// <summary>
/// Define o mapeamento de conta, suas restrições e relações no banco de dados.
/// </summary>
public sealed class ContaConfiguration : IEntityTypeConfiguration<Conta>
{
    /// <summary>
    /// Configura tabela, propriedades e restrições de conta no modelo do banco.
    /// </summary>
    /// <param name="builder">
    /// Construtor usado para configurar o mapeamento da entidade.
    /// </param>
    /// <remarks>
    /// Nas expressões lambda, <c>c</c> representa a conta.
    /// Nas expressões lambda, <c>u</c> representa o usuário.
    /// Nas expressões lambda, <c>t</c> representa o tipo de conta.
    /// </remarks>
    public void Configure(EntityTypeBuilder<Conta> builder)
    {
        builder.ToTable("Contas");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.SaldoInicial)
            .HasPrecision(18, 2);

        /*
         * Converte DateOnly do domínio para DateTime
         * durante a persistência no MySQL.
         *
         * No banco, a coluna continua sendo do tipo DATE.
         */
        var dateOnlyConverter =
            new ValueConverter<DateOnly, DateTime>(
                dateOnly =>
                    dateOnly.ToDateTime(TimeOnly.MinValue),

                dateTime =>
                    DateOnly.FromDateTime(dateTime));

        builder.Property(c => c.DataCriacao)
            .HasConversion(dateOnlyConverter)
            .HasColumnType("date")
            .IsRequired();

        // Usuario 1:N Conta
        builder.HasOne(c => c.Usuario)
            .WithMany(u => u.Contas)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // TipoConta 1:N Conta
        builder.HasOne(c => c.TipoConta)
            .WithMany(t => t.Contas)
            .HasForeignKey(c => c.TipoContaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}