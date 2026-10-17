using GFinancas.Domain.Entities;
using GFinancas.Domain.Enums;

namespace GFinancas.Application.Interfaces.Repository;

/// <summary>
/// Define as operações de persistência e consulta de transações.
/// </summary>
public interface ITransacaoRepository
{
    /// <summary>
    /// Busca transação pelo identificador e retorna nulo quando não encontra o registro.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    Task<Transacao?> ObterPorIdAsync(int id);

    /// <summary>
    /// Lista todos os registros de transações, ordenados por data decrescente.
    /// </summary>
    Task<IEnumerable<Transacao>> ObterTodasAsync();


    Task<decimal> ObterTotalDespesasPorCategoriaAsync(int usuarioId,int categoriaId,DateTime dataInicio,DateTime dataFim);

    /// <summary>
    /// Adiciona transação ao contexto e salva as alterações no banco.
    /// </summary>
    /// <param name="transacao">Transação utilizada na operação.</param>
    Task AdicionarAsync(Transacao transacao);

    /// <summary>
    /// Atualiza a transação no contexto e salva as alterações no banco.
    /// </summary>
    /// <param name="transacao">Transação utilizada na operação.</param>
    Task AtualizarAsync(Transacao transacao);

    /// <summary>
    /// Remove a transação do contexto e salva a exclusão no banco.
    /// </summary>
    /// <param name="transacao">Transação utilizada na operação.</param>
    Task RemoverAsync(Transacao transacao);

    /// <summary>
    /// Lista as transações vinculadas à conta informada.
    /// </summary>
    /// <param name="contaId">Identificador da conta financeira.</param>
    Task<IEnumerable<Transacao>> ObterPorContaIdAsync(int contaId);

    /// <summary>
    /// Consulta transações do usuário informado, aplicando os filtros opcionais e ordenando por data decrescente.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário cujos dados serão consultados.</param>
    /// <param name="dataInicio">Limite inicial inclusivo da consulta; nulo para não aplicar este limite.</param>
    /// <param name="dataFim">Limite final inclusivo da consulta; nulo para não aplicar este limite.</param>
    /// <param name="tipo">Tipo de movimentação: receita ou despesa.</param>
    /// <param name="contaId">Identificador da conta financeira.</param>
    /// <param name="categoriaId">Identificador da categoria.</param>
    Task<IEnumerable<Transacao>> FiltrarAsync(
    int usuarioId,
    DateTime? dataInicio,
    DateTime? dataFim,
    TipoTransacao? tipo,
    int? contaId,
    int? categoriaId);

    /// <summary>
    /// Retorna uma página de transações do usuário e o total de registros da consulta.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário cujos dados serão consultados.</param>
    /// <param name="pagina">Número da página solicitada, começando em 1.</param>
    /// <param name="tamanhoPagina">Quantidade de itens solicitada por página.</param>
    Task<(IEnumerable<Transacao> Itens, int TotalItens)>
    ObterPaginadoAsync(
        int usuarioId,
        int pagina,
        int tamanhoPagina);

    /// <summary>
    /// Soma todas as receitas do usuário até a data limite informada; rejeita contas inexistentes.
    /// </summary>
    /// <param name="contaId"></param>
    /// <param name="dataLimite"></param>
    /// <returns></returns>
    Task<decimal> ObterTotalReceitasAteDataAsync(int contaId,DateTime dataLimite);

    /// <summary>
    /// Soma todas as despesas do usuário até a data limite informada; rejeita contas inexistentes.
    /// </summary>
    /// <param name="contaId"></param>
    /// <param name="dataLimite"></param>
    /// <returns></returns>
    Task<decimal> ObterTotalDespesasAteDataAsync(int contaId,DateTime dataLimite);

    
}
