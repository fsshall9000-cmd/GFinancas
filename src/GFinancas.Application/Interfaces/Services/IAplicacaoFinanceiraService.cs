using GFinancas.Application.DTOs.Aplicacoes;

namespace GFinancas.Application.Interfaces.Services;

/// <summary>
/// Define as operações e os indicadores das aplicações financeiras do usuário autenticado.
/// </summary>
public interface IAplicacaoFinanceiraService
{
    /// <summary>
    /// Valida os dados e cadastra uma aplicação ativa com valor inicial zero para o usuário autenticado.
    /// </summary>
    /// <param name="dto">Dados recebidos para a operação.</param>
    Task<AplicacaoFinanceiraResponseDto> CriarAsync(
        CriarAplicacaoFinanceiraDto dto);

    /// <summary>
    /// Lista as aplicações financeiras do usuário.
    /// </summary>
    Task<IEnumerable<AplicacaoFinanceiraResponseDto>>
        ListarAsync();

    /// <summary>
    /// Busca a aplicação pelo identificador, restringindo a consulta ao usuário.
    /// </summary>
    /// <param name="id">Identificador da aplicação financeira.</param>
    Task<AplicacaoFinanceiraResponseDto>
        ObterPorIdAsync(int id);

    /// <summary>
    /// Atualiza os dados da aplicação financeira.
    /// </summary>
    /// <param name="id">Identificador da aplicação financeira.</param>
    /// <param name="dto">Dados recebidos para a operação.</param>
    Task<AplicacaoFinanceiraResponseDto>
        AtualizarAsync(
            int id,
            AtualizarAplicacaoFinanceiraDto dto);

    /// <summary>
    /// Desativa uma aplicação do usuário autenticado, rejeitando registros inexistentes ou já desativados.
    /// </summary>
    /// <param name="id">Identificador da aplicação financeira.</param>
    Task DesativarAsync(int id);

    /// <summary>
    /// Atualiza o valor da aplicação financeira, criando um novo registro de movimentação com a data de referência informada.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<AplicacaoFinanceiraResponseDto> AtualizarValorAsync(int id,AtualizarValorAplicacaoDto dto);

    /// <summary>
    /// Lista o histórico de valores da aplicação financeira, ordenando do mais recente para o mais antigo.
    /// </summary>
    /// <param name="aplicacaoId"></param>
    /// <returns></returns>
    Task<IEnumerable<HistoricoValorAplicacaoResponseDto>> ListarHistoricoValoresAsync(int aplicacaoId);
}