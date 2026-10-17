namespace GFinancas.Application.DTOs.Dashboard;

public class EvolucaoPatrimonialDto
{
    public int Mes { get; set; }

    public string MesNome { get; set; } = string.Empty;

    public decimal SaldoContas { get; set; }

    public decimal ValorInvestimentos { get; set; }

    public decimal PatrimonioTotal { get; set; }
}   