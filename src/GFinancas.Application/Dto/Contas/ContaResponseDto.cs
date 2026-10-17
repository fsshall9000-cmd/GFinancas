namespace GFinancas.Application.DTOs.Contas;

/// <summary>
/// Transporta os dados de conta devolvidos pela aplicação.
/// </summary>
public class ContaResponseDto
{
    /// <summary>
    /// Identificador único do registro.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome usado para identificar o registro na aplicação.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Valor de partida da conta, anterior às transações registradas.
    /// </summary>
    public decimal SaldoInicial { get; set; }

    /// <summary>
    /// Identificador do usuário associado ao registro.
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Nome de exibição do usuário proprietário.
    /// </summary>
    public string Usuario { get; set; } = string.Empty;

    /// <summary>
    /// Identificador da modalidade da conta financeira.
    /// </summary>
    public int TipoContaId { get; set; }

    /// <summary>
    /// Nome de exibição do tipo de conta vinculado.
    /// </summary>
    public string TipoConta { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação do registro.
    /// </summary>
    public DateOnly DataCriacao { get; set; }
}
