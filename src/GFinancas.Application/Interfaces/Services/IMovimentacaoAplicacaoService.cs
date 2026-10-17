using GFinancas.Application.DTOs.Aplicacoes;

namespace GFinancas.Application.Interfaces.Services;

/// <summary>
/// Define o registro e a consulta de movimentações de aplicações do usuário autenticado.
/// </summary>
public interface IMovimentacaoAplicacaoService
{
    /// <summary>
    /// Valida a titularidade, a aplicação ativa e o saldo disponível e grava a movimentação e o novo valor da aplicação em uma transação.
    /// </summary>
    /// <param name="dto">Dados recebidos para a operação.</param>
    Task<MovimentacaoAplicacaoResponseDto> RegistrarAsync(
        CriarMovimentacaoAplicacaoDto dto);

    /// <summary>
    /// Verifica se a aplicação pertence ao usuário autenticado e retorna seu histórico de movimentações.
    /// </summary>
    /// <param name="aplicacaoId">Identificador da aplicação financeira.</param>
    Task<IEnumerable<MovimentacaoAplicacaoResponseDto>>
        ListarPorAplicacaoAsync(int aplicacaoId);
}