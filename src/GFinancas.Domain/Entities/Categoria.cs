using GFinancas.Domain.Enums;

namespace GFinancas.Domain.Entities;

/// <summary>
/// Classifica transações de um usuário como receitas ou despesas.
/// </summary>
public class Categoria
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
    /// Natureza da movimentação: receita (1) ou despesa (2).
    /// </summary>
    public TipoTransacao Tipo { get; set; }

    // FK Usuario
    /// <summary>
    /// Identificador do usuário associado ao registro.
    /// </summary>
    public int UsuarioId { get; set; }
    /// <summary>
    /// Propriedade de navegação para os dados do usuário proprietário.
    /// </summary>
    public Usuario? Usuario { get; set; }

    /// <summary>
    /// Movimentações financeiras vinculadas a este cadastro.
    /// </summary>
    public ICollection<Transacao> Transacoes { get; set; }
        = new List<Transacao>();


}
