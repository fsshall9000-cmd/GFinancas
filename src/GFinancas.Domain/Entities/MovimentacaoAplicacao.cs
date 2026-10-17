using GFinancas.Domain.Enums;

namespace GFinancas.Domain.Entities;

/// <summary>
/// Representa um aporte ou resgate que relaciona uma aplicação financeira a uma conta.
/// </summary>
public class MovimentacaoAplicacao
{
    public int Id { get; set; }

    public decimal Valor { get; set; }

    public DateTime Data { get; set; }

    public TipoMovimentacaoAplicacao Tipo { get; set; }

    public int AplicacaoFinanceiraId { get; set; }

    public AplicacaoFinanceira AplicacaoFinanceira { get; set; }
        = null!;

    public int ContaId { get; set; }

    public Conta Conta { get; set; } = null!;
}