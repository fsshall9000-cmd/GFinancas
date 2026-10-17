using GFinancas.Application.Dto.Transacoes;
using GFinancas.Application.DTOs.Transacoes;
using GFinancas.Application.DTOs.Comum;

namespace GFinancas.Application.Interfaces.Services;

/// <summary>
/// Define as operações de aplicação disponíveis para transações.
/// </summary>
public interface ITransacaoService
{
    /// <summary>
    /// Valida os dados e a propriedade da conta e da categoria antes de cadastrar a transação.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    Task<TransacaoResponseDto> CriarAsync(CriarTransacaoDto dto);
    /// <summary>
    /// Lista transações cadastrados no sistema e converte os registros em DTOs de resposta.
    /// </summary>
    Task<IEnumerable<TransacaoResponseDto>>ListarAsync();
    /// <summary>
    /// Busca transação pelo identificador e verifica se pertence ao usuário atual.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    Task<TransacaoResponseDto?> ObterPorIdAsync(int id);
    /// <summary>
    /// Valida o usuário e os filtros informados e retorna as transações correspondentes.
    /// </summary>
    /// <param name="filtro">Usuário e critérios de seleção das transações.</param>
    Task<IEnumerable<TransacaoResponseDto>> FiltrarAsync(FiltroTransacaoDto filtro);
    /// <summary>
    /// Valida a transação e os vínculos do usuário atual antes de salvar os novos dados.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    Task<TransacaoResponseDto> AtualizarAsync(int id,AtualizarTransacaoDto dto);
    /// <summary>
    /// Verifica se a transação pertence ao usuário atual antes de removê-la.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    Task RemoverAsync(int id);
    /// <summary>
    /// Consulta as transações do usuário atual e monta os itens e metadados de paginação.
    /// </summary>
    /// <param name="pagina">Número da página solicitada, começando em 1.</param>
    /// <param name="tamanhoPagina">Quantidade de itens solicitada por página.</param>
    Task<ResultadoPaginadoDto<TransacaoResponseDto>>ListarPaginadoAsync(int pagina, int tamanhoPagina);

}
