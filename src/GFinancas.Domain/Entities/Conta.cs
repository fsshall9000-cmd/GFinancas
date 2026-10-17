namespace GFinancas.Domain.Entities;

/// <summary>
/// Representa uma conta financeira de um usuário, com saldo inicial e tipo de conta.
/// </summary>
public class Conta
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
    /// Data em que a conta passou a fazer parte do patrimônio do usuário.
    /// </summary>
    public DateOnly DataCriacao { get; set; }

    // FK Usuario

    /// <summary>
    /// Identificador do usuário associado ao registro.
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Propriedade de navegação para os dados do usuário proprietário.
    /// </summary>
    public Usuario? Usuario { get; set; }

    // FK TipoConta

    /// <summary>
    /// Identificador da modalidade da conta financeira.
    /// </summary>
    public int TipoContaId { get; set; }

    /// <summary>
    /// Propriedade de navegação para os dados do tipo de conta vinculado.
    /// </summary>
    public TipoConta? TipoConta { get; set; }

    /// <summary>
    /// Movimentações financeiras vinculadas a este cadastro.
    /// </summary>
    public ICollection<Transacao> Transacoes { get; set; }
        = new List<Transacao>();
}