using GFinancas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFinancas.Infrastructure.Data.Configurations;

/// <summary>
/// Define o mapeamento de categoria, suas restrições e relações no banco de dados.
/// </summary>
public sealed class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
	/// <summary>
	/// Configura tabela, propriedades e restrições de categoria no modelo do banco.
	/// </summary>
	/// <param name="builder">Construtor usado para configurar o mapeamento da entidade.</param>
	/// <remarks>
	/// Nas expressões lambda, <c>c</c> representa a categoria na expressão de consulta ou mapeamento.
	/// Nas expressões lambda, <c>u</c> representa o usuário na expressão de consulta ou mapeamento.
	/// </remarks>
	public void Configure(EntityTypeBuilder<Categoria> builder)
	{
		builder.ToTable("Categorias");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Tipo)
            .IsRequired();

        // Usuario 1:N Categoria
        builder.HasOne(c => c.Usuario)
            .WithMany(u => u.Categorias)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new
        {
            c.UsuarioId,
            c.Nome,
            c.Tipo
        }).IsUnique();
	}
}
