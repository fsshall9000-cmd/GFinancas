using GFinancas.Application.DTOs.Dashboard;
using GFinancas.Application.Exceptions;
using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Application.Interfaces.Repository;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Application.Interfaces.Services;
using GFinancas.Domain.Enums;
using System.Globalization;

namespace GFinancas.Application.Services;

/// <summary>
/// Calcula os indicadores financeiros do usuário atual por mês, categoria e ano.
/// </summary>
public class DashboardService : IDashboardService
{

    private readonly
    IAplicacaoFinanceiraRepository
        _aplicacaoRepository;

    private readonly ISaldoContaService _saldoContaService;
    private readonly IContaRepository _contaRepository;
    private readonly IHistoricoValorAplicacaoRepository _historicoValorAplicacaoRepository;

private readonly
    IMovimentacaoAplicacaoRepository
        _movimentacaoAplicacaoRepository;
    /// <summary>
    /// Serviço que fornece o identificador do usuário da requisição atual.
    /// </summary>
    private readonly IUsuarioAtualService _usuarioAtualService;
    /// <summary>
    /// Repositório usado para acessar os dados de transações.
    /// </summary>
    private readonly ITransacaoRepository _transacaoRepository;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="transacaoRepository">Repositório usado para acessar os dados de transações.</param>
    /// <param name="usuarioAtualService">Serviço que fornece o identificador do usuário da requisição atual.</param>
    public DashboardService(
        ITransacaoRepository transacaoRepository,
        IUsuarioAtualService usuarioAtualService,
        IAplicacaoFinanceiraRepository aplicacaoRepository,
        IMovimentacaoAplicacaoRepository movimentacaoAplicacaoRepository,
        ISaldoContaService saldoContaService,
        IContaRepository contaRepository,
        IHistoricoValorAplicacaoRepository historicoValorAplicacaoRepository)
    {
        _transacaoRepository = transacaoRepository;
        _usuarioAtualService = usuarioAtualService;
        _aplicacaoRepository = aplicacaoRepository;
        _movimentacaoAplicacaoRepository = movimentacaoAplicacaoRepository;
        _saldoContaService = saldoContaService;
        _contaRepository = contaRepository;
        _historicoValorAplicacaoRepository = historicoValorAplicacaoRepository;
    }

    /// <summary>
    /// Soma receitas e despesas do usuário atual no mês e calcula o resultado financeiro.
    /// </summary>
    /// <param name="ano">Ano de referência da consulta.</param>
    /// <param name="mes">Mês de referência da consulta, de 1 a 12.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>t</c> representa a transação na expressão de consulta, cálculo ou projeção.
    /// </remarks>
    public async Task<ResumoFinanceiroDto> ObterResumoAsync(int ano, int mes)
    {
        // dataInicio e dataFim delimitam o período, incluindo seu primeiro e último instante.
        var (dataInicio, dataFim) = ObterPeriodo(ano, mes);
        // Transações obtidas na consulta para compor a resposta ou calcular os indicadores.
        var transacoes = (await _transacaoRepository.FiltrarAsync(
            _usuarioAtualService.UsuarioId,
            dataInicio,
            dataFim,
            null,
            null,
            null)).ToList();

        // Soma dos valores das transações classificadas como receita.
        var totalReceitas = transacoes
            .Where(t => t.Tipo == TipoTransacao.Receita)
            .Sum(t => t.Valor);

        // Soma dos valores das transações classificadas como despesa.
        var totalDespesas = transacoes
            .Where(t => t.Tipo == TipoTransacao.Despesa)
            .Sum(t => t.Valor);

        return new ResumoFinanceiroDto
        {
            Ano = ano,
            Mes = mes,
            TotalReceitas = totalReceitas,
            TotalDespesas = totalDespesas,
            ResultadoMes = totalReceitas - totalDespesas,
            QuantidadeTransacoes = transacoes.Count
        };
    }

    /// <summary>
    /// Agrupa as despesas mensais do usuário atual por categoria e calcula sua participação percentual.
    /// </summary>
    /// <param name="ano">Ano de referência da consulta.</param>
    /// <param name="mes">Mês de referência da consulta, de 1 a 12.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>t</c> representa a transação na expressão de consulta, cálculo ou projeção.
    /// Nas expressões lambda, <c>g</c> representa o grupo de despesas de uma categoria.
    /// Nas expressões lambda, <c>x</c> representa o total agregado de uma categoria para ordenação.
    /// </remarks>
    public async Task<IEnumerable<DespesaPorCategoriaDto>> ObterDespesasPorCategoriaAsync(
        int ano,
        int mes)
    {
        // dataInicio e dataFim delimitam o período, incluindo seu primeiro e último instante.
        var (dataInicio, dataFim) = ObterPeriodo(ano, mes);
        // Transações obtidas na consulta para compor a resposta ou calcular os indicadores.
        var transacoes = await _transacaoRepository.FiltrarAsync(
            _usuarioAtualService.UsuarioId,
            dataInicio,
            dataFim,
            TipoTransacao.Despesa,
            null,
            null);

        // Lista das transações de despesa do período, usada no agrupamento por categoria.
        var despesas = transacoes.ToList();
        // Soma dos valores das transações classificadas como despesa.
        var totalDespesas = despesas.Sum(t => t.Valor);

        return despesas
            .GroupBy(t => new
            {
                t.CategoriaId,
                Nome = t.Categoria?.Nome ?? "Sem categoria"
            })
            .Select(g => new DespesaPorCategoriaDto
            {
                CategoriaId = g.Key.CategoriaId,
                Categoria = g.Key.Nome,
                Total = g.Sum(t => t.Valor),
                Percentual = totalDespesas == 0 ? 0 : g.Sum(t => t.Valor) / totalDespesas * 100
            })
            .OrderByDescending(x => x.Total)
            .ToList();
    }

    /// <summary>
    /// Produz os totais dos doze meses do ano para o usuário atual, incluindo meses sem movimentação.
    /// </summary>
    /// <param name="ano">Ano de referência da consulta.</param>
    /// <remarks>
    /// Nas expressões lambda, <c>t</c> representa a transação na expressão de consulta, cálculo ou projeção.
    /// Nas expressões lambda, <c>mes</c> percorre os meses de 1 a 12 para montar o histórico anual.
    /// </remarks>
    public async Task<IEnumerable<HistoricoMensalDto>> ObterHistoricoMensalAsync(int ano)
    {
        // dataInicio e dataFim delimitam o período, incluindo seu primeiro e último instante.
        var (dataInicio, dataFim) = ObterPeriodo(ano);
        // Transações obtidas na consulta para compor a resposta ou calcular os indicadores.
        var transacoes = await _transacaoRepository.FiltrarAsync(
            _usuarioAtualService.UsuarioId,
            dataInicio,
            dataFim,
            null,
            null,
            null);

        // Agrupa as transações pelo número do mês para montar o histórico anual.
        var transacoesPorMes = transacoes.ToLookup(t => t.Data.Month);

        return Enumerable.Range(1, 12)
            .Select(mes =>
            {
                // Transações correspondentes ao mês atual da projeção do histórico.
                var transacoesMes = transacoesPorMes[mes];
                // Total das receitas do mês em processamento.
                var receitas = transacoesMes
                    .Where(t => t.Tipo == TipoTransacao.Receita)
                    .Sum(t => t.Valor);

                // Total das despesas do mês em processamento.
                var despesas = transacoesMes
                    .Where(t => t.Tipo == TipoTransacao.Despesa)
                    .Sum(t => t.Valor);

                return new HistoricoMensalDto
                {
                    Mes = mes,
                    MesNome = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetMonthName(mes),
                    TotalReceitas = receitas,
                    TotalDespesas = despesas,
                    Resultado = receitas - despesas
                };
            })
            .ToList();
    }

    public async Task<ResumoInvestimentosDto>
    ObterResumoInvestimentosAsync()
{
    var usuarioId =
        _usuarioAtualService.UsuarioId;

    var aplicacoes =
        await _aplicacaoRepository
            .ListarAsync(usuarioId);

    var lista =
        aplicacoes.ToList();

    decimal totalAportado = 0;

    decimal totalResgatado = 0;

    decimal valorAtual = 0;

    foreach (var aplicacao in lista)
    {
        var aportes =
            await _movimentacaoAplicacaoRepository
                .ObterTotalAportesAsync(
                    aplicacao.Id);

        var resgates =
            await _movimentacaoAplicacaoRepository
                .ObterTotalResgatesAsync(
                    aplicacao.Id);

        totalAportado += aportes;

        totalResgatado += resgates;

        valorAtual +=
            aplicacao.ValorAtual;
    }

    var resultado =
        valorAtual
        + totalResgatado
        - totalAportado;

    var rentabilidadePercentual =
        totalAportado > 0
            ? resultado
              / totalAportado
              * 100
            : 0;

    return new ResumoInvestimentosDto
    {
        TotalAportado =
            totalAportado,

        TotalResgatado =
            totalResgatado,

        ValorAtual =
            valorAtual,

        Resultado =
            resultado,

        RentabilidadePercentual =
            rentabilidadePercentual,

        QuantidadeAplicacoesAtivas =
            lista.Count(a => a.Ativa)
    };
}
    public async Task<ResumoPatrimonioDto>
    ObterPatrimonioAsync()
{
    var usuarioId =
        _usuarioAtualService.UsuarioId;

    var contas =
        await _contaRepository
            .ObterPorUsuarioIdAsync(usuarioId);

    decimal saldoContas = 0;

    foreach (var conta in contas)
    {
        var saldo =
            await _saldoContaService
                .CalcularAsync(conta.Id);

        saldoContas += saldo;
    }


    var aplicacoes =
        await _aplicacaoRepository
            .ListarAsync(usuarioId);

    var valorInvestimentos =
        aplicacoes.Sum(
            aplicacao => aplicacao.ValorAtual
        );


    var patrimonioTotal =
        saldoContas +
        valorInvestimentos;


    return new ResumoPatrimonioDto
    {
        SaldoContas = saldoContas,

        ValorInvestimentos =
            valorInvestimentos,

        PatrimonioTotal =
            patrimonioTotal
    };
}




    /// <summary>
/// Calcula o saldo das contas e o último valor registrado
/// dos investimentos ao fim de cada mês.
/// </summary>
public async Task<IEnumerable<EvolucaoPatrimonialDto>>
    ObterEvolucaoPatrimonialAsync(int ano)
{
    ObterPeriodo(ano);

    var hoje = DateTime.UtcNow;

    if (ano > hoje.Year)
    {
        throw new ValidacaoException(
            "Não é possível consultar a evolução patrimonial de um ano futuro.");
    }

    var usuarioId =
        _usuarioAtualService.UsuarioId;

    var contas =
        (await _contaRepository
            .ObterPorUsuarioIdAsync(usuarioId))
        .ToList();

    var aplicacoes =
        (await _aplicacaoRepository
            .ListarAsync(usuarioId))
        .ToList();

    var evolucao =
        new List<EvolucaoPatrimonialDto>();

    var ultimoMes =
        ano == hoje.Year
            ? hoje.Month
            : 12;

    for (var mes = 1; mes <= ultimoMes; mes++)
    {
        DateTime dataLimite;
        DateOnly dataReferencia;

        var ehMesAtual =
            ano == hoje.Year &&
            mes == hoje.Month;

        if (ehMesAtual)
        {
            dataLimite = hoje;

            dataReferencia =
                DateOnly.FromDateTime(hoje);
        }
        else
        {
            var (_, fimMes) =
                ObterPeriodo(ano, mes);

            dataLimite = fimMes;

            dataReferencia =
                DateOnly.FromDateTime(fimMes);
        }

        decimal saldoContas = 0;

        foreach (var conta in contas)
        {
            saldoContas +=
                await _saldoContaService
                    .CalcularAteDataAsync(
                        conta.Id,
                        dataLimite);
        }

        decimal valorInvestimentos = 0;

        foreach (var aplicacao in aplicacoes)
        {
            var historico =
                await _historicoValorAplicacaoRepository
                    .ObterUltimoAteDataAsync(
                        aplicacao.Id,
                        usuarioId,
                        dataReferencia);

            valorInvestimentos +=
                historico?.Valor ?? 0;
        }

        evolucao.Add(
            new EvolucaoPatrimonialDto
            {
                Mes = mes,

                MesNome =
                    CultureInfo
                        .GetCultureInfo("pt-BR")
                        .DateTimeFormat
                        .GetMonthName(mes),

                SaldoContas =
                    saldoContas,

                ValorInvestimentos =
                    valorInvestimentos,

                PatrimonioTotal =
                    saldoContas +
                    valorInvestimentos
            });
    }

    return evolucao;
}

    /// <summary>
    /// Valida ano e mês e calcula os limites inclusivos do período mensal ou anual.
    /// </summary>
    /// <param name="ano">Ano de referência da consulta.</param>
    /// <param name="mes">Mês de referência; quando opcional e nulo, considera o ano inteiro.</param>
    private static (DateTime Inicio, DateTime Fim) ObterPeriodo(int ano, int? mes = null)
    {
        if (ano < 1 || ano > 9999)
            throw new ValidacaoException("O ano deve estar entre 1 e 9999.");

        if (mes.HasValue && (mes.Value < 1 || mes.Value > 12))
            throw new ValidacaoException("O mês deve estar entre 1 e 12.");

        // Primeiro dia do mês solicitado, ou primeiro dia do ano quando o mês não é informado.
        var inicio = new DateTime(ano, mes ?? 1, 1);
        // Mês solicitado, ou dezembro para uma consulta anual.
        var ultimoMes = mes ?? 12;
        // Último dia do período, ainda à meia-noite.
        var ultimoDia = new DateTime(ano, ultimoMes, DateTime.DaysInMonth(ano, ultimoMes));

        // O repositório considera a data final inclusiva.
        // Último instante do último dia, para incluir todo o período na consulta.
        var fim = ultimoDia.AddTicks(TimeSpan.TicksPerDay - 1);
        return (inicio, fim);
    }
}
