using GFinancas.Domain.Entities;
using GFinancas.Domain.Enums;

namespace GFinancas.Application.Interfaces.Repositories;

/// <summary>
/// Define as operações de persistência e consulta de categorias.
/// </summary>
public interface ICategoriaRepository
{
    /// <summary>
    /// Adiciona categoria ao contexto e salva as alterações no banco.
    /// </summary>
    /// <param name="categoria">Categoria utilizada na operação.</param>
    Task AdicionarAsync(Categoria categoria);

    /// <summary>
    /// Busca categoria pelo identificador e retorna nulo quando não encontra o registro.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    Task<Categoria?> ObterPorIdAsync(int id);

    /// <summary>
    /// Lista todos os registros de categorias, ordenados por nome.
    /// </summary>
    Task<IEnumerable<Categoria>> ObterTodasAsync();

    /// <summary>
    /// Busca uma categoria pela combinação de nome, tipo e proprietário.
    /// </summary>
    /// <param name="nome">Nome da categoria procurada.</param>
    /// <param name="tipo">Tipo de movimentação: receita ou despesa.</param>
    /// <param name="usuarioId">Identificador do usuário cujos dados serão consultados.</param>
    Task<Categoria?> ObterPorNomeTipoEUsuarioAsync(string nome,TipoTransacao tipo,int usuarioId);
    /// <summary>
    /// Lista categorias do usuário informado, em ordem de nome.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário cujos dados serão consultados.</param>
    Task<IEnumerable<Categoria>> ObterPorUsuarioIdAsync(int usuarioId);


    /// <summary>Atualiza a categoria e salva as alterações.</summary>
    Task AtualizarAsync(Categoria categoria);

    /// <summary>Remove a categoria e salva as alterações.</summary>
    Task RemoverAsync(Categoria categoria);

    /// <summary>Verifica se existem transações vinculadas à categoria.</summary>
    Task<bool> PossuiTransacoesAsync(int categoriaId);
}