using GFinancas.Application.DTOs.Categorias;

namespace GFinancas.Application.Interfaces.Services;

/// <summary>
/// Define as operações de aplicação disponíveis para categorias.
/// </summary>
public interface ICategoriaService
{
    /// <summary>
    /// Valida nome, tipo e duplicidade e cadastra a categoria para o usuário atual.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    Task<CategoriaResponseDto> CriarAsync(
        CriarCategoriaDto dto);

    /// <summary>
    /// Lista categorias do usuário atual e converte os registros em DTOs de resposta.
    /// </summary>
    Task<IEnumerable<CategoriaResponseDto>> ListarAsync();

    /// <summary>
    /// Busca categoria pelo identificador e verifica se pertence ao usuário atual.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    Task<CategoriaResponseDto?> ObterPorIdAsync(int id);

    /// <summary>Atualiza uma categoria do usuário atual.</summary>
    Task<CategoriaResponseDto> AtualizarAsync(int id, AtualizarCategoriaDto dto);

    /// <summary>Remove uma categoria do usuário atual sem transações vinculadas.</summary>
    Task RemoverAsync(int id);
}