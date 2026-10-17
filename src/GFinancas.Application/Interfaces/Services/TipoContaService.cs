using GFinancas.Application.DTOs.TiposConta;
using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Application.Interfaces.Services;

namespace GFinancas.Application.Services;

/// <summary>
/// Executa as operações de aplicação relacionadas aos tipos de conta.
/// </summary>
public class TipoContaService : ITipoContaService
{
    private readonly ITipoContaRepository _tipoContaRepository;

    public TipoContaService(
        ITipoContaRepository tipoContaRepository)
    {
        _tipoContaRepository = tipoContaRepository;
    }

    /// <summary>
    /// Lista os tipos de conta disponíveis.
    /// </summary>
    public async Task<IEnumerable<TipoContaResponseDto>> ListarAsync()
    {
        var tiposConta =
            await _tipoContaRepository.ObterTodosAsync();

        return tiposConta.Select(t =>
            new TipoContaResponseDto
            {
                Id = t.Id,
                Nome = t.Nome
            });
    }
}