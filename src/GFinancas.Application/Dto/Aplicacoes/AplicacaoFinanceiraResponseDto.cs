namespace GFinancas.Application.DTOs.Aplicacoes;

/// <summary>
/// Transporta os dados da aplicação e seus indicadores de capital, resultado e rentabilidade.
/// </summary>
public class AplicacaoFinanceiraResponseDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public int Tipo { get; set; }

    public string TipoDescricao { get; set; } = string.Empty;

    public string Instituicao { get; set; } = string.Empty;

    public DateTime DataInicio { get; set; }

    public decimal TotalAportado { get; set; }

    public decimal TotalResgatado { get; set; }

    public decimal CapitalLiquido { get; set; }

    public decimal ValorAtual { get; set; }

    public decimal Resultado { get; set; }

    public decimal RentabilidadePercentual { get; set; }

    public bool Ativa { get; set; }
}