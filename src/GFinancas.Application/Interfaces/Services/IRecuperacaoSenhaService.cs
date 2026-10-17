using GFinancas.Application.DTOs.Auth;

namespace GFinancas.Application.Interfaces.Services;

public interface IRecuperacaoSenhaService
{
    Task SolicitarAsync(RecuperarSenhaDto dto);
    Task RedefinirAsync(RedefinirSenhaDto dto);
}
