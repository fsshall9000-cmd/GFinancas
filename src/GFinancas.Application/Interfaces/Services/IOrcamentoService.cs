using GFinancas.Application.DTOs.Orcamentos;

namespace GFinancas.Application.Interfaces.Services;

public interface IOrcamentoService
{
    Task<OrcamentoResponseDto> CriarAsync(
        CriarOrcamentoDto dto);

    Task<IEnumerable<OrcamentoResponseDto>> ListarPorMesAsync(
        int ano,
        int mes);

    Task<OrcamentoResponseDto> ObterPorIdAsync(
        int id);

    Task<OrcamentoResponseDto> AtualizarAsync(
        int id,
        AtualizarOrcamentoDto dto);

    Task RemoverAsync(
        int id);
}