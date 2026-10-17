using GFinancas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFinancas.Infrastructure.Data.Configurations;

public class OrcamentoConfiguration : IEntityTypeConfiguration<Orcamento>
{
    public void Configure(
        EntityTypeBuilder<Orcamento> builder)
    {
        builder.ToTable("Orcamentos");

        builder.HasKey(o => o.Id);


        // ==========================================
        // VALOR LIMITE
        // ==========================================

        builder.Property(o => o.ValorLimite)
            .HasPrecision(18, 2)
            .IsRequired();


        // ==========================================
        // ANO
        // ==========================================

        builder.Property(o => o.Ano)
            .IsRequired();


        // ==========================================
        // MÊS
        // ==========================================

        builder.Property(o => o.Mes)
            .IsRequired();


        // ==========================================
        // RELAÇÃO COM USUÁRIO
        // ==========================================

        builder.HasOne(o => o.Usuario)
            .WithMany()
            .HasForeignKey(o => o.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);


        // ==========================================
        // RELAÇÃO COM CATEGORIA
        // ==========================================

        builder.HasOne(o => o.Categoria)
            .WithMany()
            .HasForeignKey(o => o.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);


        // ==========================================
        // ÍNDICE ÚNICO
        // ==========================================

        builder.HasIndex(o => new
        {
            o.UsuarioId,
            o.CategoriaId,
            o.Ano,
            o.Mes
        })
        .IsUnique();
    }
}