using GFinancas.Domain.Enums;

namespace GFinancas.Domain.Entities;

public class AplicacaoFinanceira
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public TipoAplicacao Tipo { get; set; }

    public string Instituicao { get; set; } = string.Empty;

    public DateTime DataInicio { get; set; }

    public decimal ValorAtual { get; set; }

    public bool Ativa { get; set; } = true;


    // =====================================================
    // USUÁRIO
    // =====================================================

    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;


    // =====================================================
    // MOVIMENTAÇÕES
    // =====================================================

    public ICollection<MovimentacaoAplicacao> Movimentacoes
        { get; set; }
        = new List<MovimentacaoAplicacao>();


    // =====================================================
    // HISTÓRICO DE VALORES
    // =====================================================

    public ICollection<HistoricoValorAplicacao> HistoricoValores
        { get; set; }
        = new List<HistoricoValorAplicacao>();
}