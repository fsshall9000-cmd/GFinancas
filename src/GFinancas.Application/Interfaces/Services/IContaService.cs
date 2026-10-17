using GFinancas.Application.DTOs.Contas;

namespace GFinancas.Application.Interfaces.Services;

/// <summary>
/// Define as operações de aplicação disponíveis para contas.
/// </summary>
public interface IContaService
{
    /// <summary>
    /// Valida nome e tipo de conta e cadastra a conta para o usuário atual.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    Task<ContaResponseDto> CriarAsync(CriarContaDto dto);
    /// <summary>
    /// Lista contas do usuário atual e converte os registros em DTOs de resposta.
    /// </summary>
    Task<IEnumerable<ContaResponseDto>> ListarAsync();
    /// <summary>
    /// Busca conta pelo identificador e verifica se pertence ao usuário atual.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    Task<ContaResponseDto?> ObterPorIdAsync(int id);
    /// <summary>
    /// Verifica a propriedade da conta e calcula saldo inicial mais receitas menos despesas.
    /// </summary>
    /// <param name="contaId">Identificador da conta financeira.</param>
    Task<SaldoContaResponseDto> ObterSaldoAsync(int contaId);
    Task<ContaResponseDto> AtualizarAsync(int id,AtualizarContaDto dto);
    Task RemoverAsync(int id);
}