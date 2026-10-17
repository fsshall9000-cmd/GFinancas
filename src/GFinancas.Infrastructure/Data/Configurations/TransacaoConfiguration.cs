using GFinancas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFinancas.Infrastructure.Data.Configurations;

/// <summary>
/// Define o mapeamento de transação, suas restrições e relações no banco de dados.
/// </summary>
public sealed class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
{
	/// <summary>
	/// Configura tabela, propriedades e restrições de transação no modelo do banco.
	/// </summary>
	/// <param name="builder">Construtor usado para configurar o mapeamento da entidade.</param>
	/// <remarks>
	/// Nas expressões lambda, <c>t</c> representa a transação na expressão de consulta, cálculo ou projeção.
	/// Nas expressões lambda, <c>c</c> representa a conta ou a categoria vinculada na configuração do relacionamento.
	/// </remarks>
	public void Configure(EntityTypeBuilder<Transacao> builder)
	{
		  builder.ToTable("Transacoes");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Descricao)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Valor)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.Data)
            .IsRequired();

        builder.Property(t => t.Tipo)
            .IsRequired();

        // Conta 1:N Transacao
        builder.HasOne(t => t.Conta)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.ContaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Categoria 1:N Transacao
        builder.HasOne(t => t.Categoria)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
	}
}
