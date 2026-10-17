using GFinancas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFinancas.Infrastructure.Data.Configurations;

/// <summary>
/// Define o mapeamento de usuário, suas restrições e relações no banco de dados.
/// </summary>
public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
	/// <summary>
	/// Configura tabela, propriedades e restrições de usuário no modelo do banco.
	/// </summary>
	/// <param name="builder">Construtor usado para configurar o mapeamento da entidade.</param>
	/// <remarks>
	/// Nas expressões lambda, <c>usuario</c> representa o usuário na expressão de consulta ou mapeamento.
	/// Nas expressões lambda, <c>u</c> representa o usuário na expressão de consulta ou mapeamento.
	/// </remarks>
	public void Configure(EntityTypeBuilder<Usuario> builder)
	{
		builder.ToTable("Usuarios");

		builder.HasKey(usuario => usuario.Id);

		builder.Property(usuario => usuario.Nome)
			.IsRequired()
			.HasMaxLength(150);

		builder.Property(usuario => usuario.Email)
			.IsRequired()
			.HasMaxLength(254);

		builder.HasIndex(usuario => usuario.Email)
			.IsUnique();

		builder.Property(u => u.SenhaHash)
    		.IsRequired()
    		.HasMaxLength(500);
	}
}
