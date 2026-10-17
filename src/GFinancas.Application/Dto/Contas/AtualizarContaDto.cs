namespace GFinancas.Application.DTOs.Contas;

public class AtualizarContaDto
{
    public string Nome { get; set; } = string.Empty;

    public decimal SaldoInicial { get; set; }

    public int TipoContaId { get; set; }
}