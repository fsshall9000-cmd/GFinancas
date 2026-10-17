namespace GFinancas.Domain.Entities;

/// <summary>
/// Representa um usuário cadastrado, suas credenciais e seus vínculos financeiros.
/// </summary>
public class Usuario
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
    /// Endereço de e-mail do usuário, usado no cadastro e na autenticação.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contas vinculadas a este cadastro.
    /// </summary>
    public ICollection<Conta> Contas { get; set; } = new List<Conta>();

    /// <summary>
    /// Categorias cadastradas pelo usuário.
    /// </summary>
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();

    /// <summary>
    /// Hash da senha usado na verificação das credenciais; não armazena a senha em texto puro.
    /// </summary>
    public string SenhaHash { get; set; } = string.Empty;
    
}
