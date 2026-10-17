using GFinancas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GFinancas.Infrastructure.Data.Configurations;

public class HistoricoValorAplicacaoConfiguration
    : IEntityTypeConfiguration<HistoricoValorAplicacao>
{
    public void Configure(
        EntityTypeBuilder<HistoricoValorAplicacao> builder)
    {
        builder.ToTable("HistoricosValoresAplicacoes");

        builder.HasKey(h => h.Id);


        // ---------------------------------------------
        // DateOnly -> DateTime para o MySQL
        // ---------------------------------------------

        var dateOnlyConverter =
            new ValueConverter<DateOnly, DateTime>(
                dateOnly =>
                    dateOnly.ToDateTime(TimeOnly.MinValue),

                dateTime =>
                    DateOnly.FromDateTime(dateTime));


        builder.Property(h => h.DataReferencia)
            .HasConversion(dateOnlyConverter)
            .HasColumnType("date")
            .IsRequired();


        // ---------------------------------------------
        // Valor histórico
        // ---------------------------------------------

        builder.Property(h => h.Valor)
            .HasPrecision(18, 2)
            .IsRequired();


        // ---------------------------------------------
        // Origem do histórico
        // ---------------------------------------------

        builder.Property(h => h.Origem)
            .HasConversion<int>()
            .IsRequired();


        // ---------------------------------------------
        // Relacionamento
        // ---------------------------------------------

        builder.HasOne(h => h.AplicacaoFinanceira)
            .WithMany(a => a.HistoricoValores)
            .HasForeignKey(h => h.AplicacaoFinanceiraId)
            .OnDelete(DeleteBehavior.Restrict);


        // ---------------------------------------------
        // Uma aplicação possui somente um histórico
        // por dia
        // ---------------------------------------------

        builder.HasIndex(h => new
        {
            h.AplicacaoFinanceiraId,
            h.DataReferencia
        })
        .IsUnique();
    }
}