using GFinancas.Domain.Enums;

/// <summary>
/// Transporta os dados necessários para cadastrar uma aplicação financeira.
/// </summary>
public class CriarAplicacaoFinanceiraDto
{
    public string Nome { get; set; } = string.Empty;

    public TipoAplicacao Tipo { get; set; }

    public string Instituicao { get; set; } = string.Empty;

    public DateTime DataInicio { get; set; }
}