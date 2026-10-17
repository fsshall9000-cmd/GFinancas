namespace GFinancas.Application.DTOs.Contas;

/// <summary>
/// Transporta os dados necessários para cadastrar conta.
/// </summary>
public class CriarContaDto
{
    /// <summary>
    /// Nome usado para identificar o registro na aplicação.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Valor de partida da conta, anterior às transações registradas.
    /// </summary>
    public decimal SaldoInicial { get; set; }

    /// <summary>
    /// Identificador da modalidade da conta financeira.
    /// </summary>
    public int TipoContaId { get; set; }
}