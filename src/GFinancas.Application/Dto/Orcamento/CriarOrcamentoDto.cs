namespace GFinancas.Application.DTOs.Orcamentos;

public class CriarOrcamentoDto
{
    public int Ano { get; set; }

    public int Mes { get; set; }

    public decimal ValorLimite { get; set; }

    public int CategoriaId { get; set; }
}
