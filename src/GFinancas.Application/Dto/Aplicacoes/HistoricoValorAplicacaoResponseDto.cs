namespace GFinancas.Application.DTOs.Aplicacoes;

public class HistoricoValorAplicacaoResponseDto
{
    public int Id { get; set; }

    public DateOnly DataReferencia { get; set; }

    public decimal Valor { get; set; }

    public int Origem { get; set; }

    public string OrigemDescricao { get; set; } =
        string.Empty;
}