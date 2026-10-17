using GFinancas.Application.DTOs.TiposConta;

namespace GFinancas.Application.Interfaces.Services;

/// <summary>
/// Define as operações de aplicação disponíveis para tipos de conta.
/// </summary>
public interface ITipoContaService
{
    /// <summary>
    /// Lista os tipos de conta disponíveis.
    /// </summary>
    Task<IEnumerable<TipoContaResponseDto>> ListarAsync();
}