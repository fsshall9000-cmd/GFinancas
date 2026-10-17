namespace GFinancas.Domain.Entities;

public class Orcamento
{
    public int Id { get; set; }

    public int Ano { get; set; }

    public int Mes { get; set; }

    public decimal ValorLimite { get; set; }


    // Usuário dono do orçamento
    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;


    // Categoria que receberá o limite
    public int CategoriaId { get; set; }

    public Categoria Categoria { get; set; } = null!;
}