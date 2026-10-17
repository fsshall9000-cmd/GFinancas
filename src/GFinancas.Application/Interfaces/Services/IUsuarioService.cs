using GFinancas.Application.Dto.Usuarios;

namespace GFinancas.Application.Interfaces.Services;

/// <summary>
/// Define as operações de aplicação disponíveis para usuários.
/// </summary>
public interface IUsuarioService
{
    /// <summary>
    /// Valida duplicidade de e-mail e senha, gera o hash e cadastra o usuário.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    Task<UsuarioResponseDto> CriarAsync(CriarUsuarioDto dto);
    /// <summary>
    /// Lista usuários cadastrados no sistema e converte os registros em DTOs de resposta.
    /// </summary>
    Task<IEnumerable<UsuarioResponseDto>> ListarAsync();
    /// <summary>
    /// Busca usuário pelo identificador e retorna nulo quando não encontra o registro.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    Task<UsuarioResponseDto?> ObterPorIdAsync(int id);
    /// <summary>
    /// Busca e retorna os dados cadastrais do usuário autenticado.
    /// </summary>
    Task<UsuarioResponseDto> ObterUsuarioAtualAsync();
}
