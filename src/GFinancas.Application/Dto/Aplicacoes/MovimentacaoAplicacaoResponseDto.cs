namespace GFinancas.Application.DTOs.Aplicacoes;

/// <summary>
/// Transporta os dados de uma movimentação, incluindo o código e a descrição de seu tipo.
/// </summary>
public class MovimentacaoAplicacaoResponseDto
{
    public int Id { get; set; }

    public decimal Valor { get; set; }
    public string Conta { get; set; } = string.Empty;

    public DateTime Data { get; set; }

    public int Tipo { get; set; }

    public string TipoDescricao { get; set; } = string.Empty;

    public int AplicacaoFinanceiraId { get; set; }

    public int ContaId { get; set; }
}