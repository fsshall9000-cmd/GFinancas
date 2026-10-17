using GFinancas.Domain.Enums;

namespace GFinancas.Domain.Entities;

public class HistoricoValorAplicacao
{
    public int Id { get; set; }

    public DateOnly DataReferencia { get; set; }

    public decimal Valor { get; set; }

    public OrigemHistoricoAplicacao Origem { get; set; }

    public int AplicacaoFinanceiraId { get; set; }

    public AplicacaoFinanceira AplicacaoFinanceira { get; set; } = null!;
}