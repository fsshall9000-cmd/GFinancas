using GFinancas.Domain.Enums;

namespace GFinancas.Application.DTOs.Transacoes;

/// <summary>
/// Reúne o usuário e os critérios opcionais para consultar transações.
/// </summary>
public class FiltroTransacaoDto
{
    /// <summary>
    /// Identificador do usuário informado no filtro e usado pelo serviço para selecionar as transações.
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Limite inicial inclusivo do período; nulo significa ausência deste filtro.
    /// </summary>
    public DateTime? DataInicio { get; set; }

    /// <summary>
    /// Limite final inclusivo do período; nulo significa ausência deste filtro.
    /// </summary>
    public DateTime? DataFim { get; set; }

    /// <summary>
    /// Natureza da movimentação: receita (1) ou despesa (2). Nulo significa ausência deste filtro.
    /// </summary>
    public TipoTransacao? Tipo { get; set; }

    /// <summary>
    /// Identificador da conta vinculada à movimentação ou à consulta. Nulo significa ausência deste filtro.
    /// </summary>
    public int? ContaId { get; set; }

    /// <summary>
    /// Identificador da categoria usada para classificar a movimentação. Nulo significa ausência deste filtro.
    /// </summary>
    public int? CategoriaId { get; set; }
}