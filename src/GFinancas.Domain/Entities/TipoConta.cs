namespace GFinancas.Domain.Entities;
/// <summary>
/// Representa uma modalidade de conta financeira disponível para cadastro.
/// </summary>
public class TipoConta
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
    /// Contas vinculadas a este cadastro.
    /// </summary>
    public ICollection<Conta> Contas { get; set; }
        = new List<Conta>();
}
