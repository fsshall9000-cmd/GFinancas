namespace GFinancas.Application.DTOs.Orcamentos;

public class OrcamentoResponseDto
{
    public int Id { get; set; }

    public int Ano { get; set; }

    public int Mes { get; set; }

    public int CategoriaId { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public decimal ValorLimite { get; set; }

    public decimal ValorGasto { get; set; }

    public decimal ValorDisponivel { get; set; }

    public decimal PercentualUtilizado { get; set; }
}