using GFinancas.Domain.Enums;

namespace GFinancas.Application.DTOs.Aplicacoes;

/// <summary>
/// Transporta a aplicação, a conta, o valor, a data e o tipo de movimentação a registrar.
/// </summary>
public class CriarMovimentacaoAplicacaoDto
{
    public int AplicacaoFinanceiraId { get; set; }

    public int ContaId { get; set; }

    public decimal Valor { get; set; }

    public DateTime Data { get; set; }

    public TipoMovimentacaoAplicacao Tipo { get; set; }
}