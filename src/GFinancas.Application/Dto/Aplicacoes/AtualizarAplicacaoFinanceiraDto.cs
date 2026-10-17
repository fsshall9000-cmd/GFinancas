namespace GFinancas.Application.DTOs.Aplicacoes;

/// <summary>
/// Transporta o nome, a instituição e o valor atual informados para atualizar uma aplicação.
/// </summary>
public class AtualizarAplicacaoFinanceiraDto
{
    public string Nome { get; set; } = string.Empty;

    public string Instituicao { get; set; } = string.Empty;

    
}