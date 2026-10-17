namespace GFinancas.Application.DTOs.Dashboard;

public class ResumoInvestimentosDto
{
    public decimal TotalAportado { get; set; }

    public decimal TotalResgatado { get; set; }

    public decimal ValorAtual { get; set; }

    public decimal Resultado { get; set; }

    public decimal RentabilidadePercentual { get; set; }

    public int QuantidadeAplicacoesAtivas { get; set; }
}