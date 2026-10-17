import {
  Component,
  OnDestroy,
  OnInit
} from '@angular/core';

import { FormsModule } from '@angular/forms';

import {
  Subject,
  takeUntil
} from 'rxjs';

import {
  CurrencyPipe,
  DecimalPipe
} from '@angular/common';

import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterLinkActive
} from '@angular/router';

import { BaseChartDirective }
  from 'ng2-charts';

import { ChartConfiguration }
  from 'chart.js';

import { OrcamentoService }
  from '../../core/services/orcamento.service';

import { ContaService }
  from '../../core/services/conta.service';

import { DashboardService }
  from '../../core/services/dashboard.service';

import { Orcamento }
  from '../../models/orcamento';

import { ResumoFinanceiro }
  from '../../models/resumo-financeiro';

import { DespesaPorCategoria }
  from '../../models/despesa-por-categoria';

import { Conta }
  from '../../models/conta';

import { HistoricoMensal }
  from '../../models/historico-mensal';

import { ResumoInvestimentos }
  from '../../models/resumo-investimentos.model';

import { ResumoPatrimonio }
  from '../../models/resumo-patrimonio.model';

import { EvolucaoPatrimonial }
  from '../../models/evolucao-patrimonial.model';


@Component({
  selector: 'app-dashboard',

  imports: [
    FormsModule,
    CurrencyPipe,
    DecimalPipe,
    RouterLink,
    RouterLinkActive,
    BaseChartDirective
  ],

  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent
  implements OnInit, OnDestroy {


  // =========================================================
  // OPÇÕES / VISÕES DO DASHBOARD
  // =========================================================

  readonly opcoesDashboard = [
    {
      caminho: '/dashboard',
      titulo: 'Visão geral'
    },
    {
      caminho: '/dashboard/financeiro',
      titulo: 'Receitas e despesas'
    },
    {
      caminho: '/dashboard/patrimonio',
      titulo: 'Patrimônio e investimentos'
    },
    {
      caminho: '/dashboard/orcamentos',
      titulo: 'Orçamentos'
    }
  ];


  visao:
    'sintetico' |
    'financeiro' |
    'patrimonio' |
    'orcamentos'
    = 'sintetico';


  get tituloDashboard(): string {

    return {
      sintetico:
        'Visão geral',

      financeiro:
        'Receitas e despesas',

      patrimonio:
        'Patrimônio e investimentos',

      orcamentos:
        'Orçamentos'
    }[this.visao];
  }


  get descricaoDashboard(): string {

    return {
      sintetico:
        'Seus principais indicadores financeiros',

      financeiro:
        'Despesas por categoria e evolução mensal',

      patrimonio:
        'Contas, investimentos e composição do patrimônio',

      orcamentos:
        'Acompanhamento dos limites e gastos do mês'
    }[this.visao];
  }


  // =========================================================
  // DADOS DO DASHBOARD
  // =========================================================

  resumo:
    ResumoFinanceiro | null = null;

  resumoInvestimentos:
    ResumoInvestimentos | null = null;

  resumoPatrimonio:
    ResumoPatrimonio | null = null;

  despesasPorCategoria:
    DespesaPorCategoria[] = [];

  historicoMensal:
    HistoricoMensal[] = [];

  evolucaoPatrimonial:
    EvolucaoPatrimonial[] = [];

  contas:
    (Conta & { saldoAtual?: number })[] = [];

  saldoTotal = 0;


  // =========================================================
  // PERÍODO
  // =========================================================

  periodo = {
    ano:
      new Date().getFullYear(),

    mes:
      new Date().getMonth() + 1
  };


  filtroAno:
    number | null = this.periodo.ano;

  filtroMes:
    number | null = this.periodo.mes;

  erroPeriodo = '';


  readonly meses = [
    'Janeiro',
    'Fevereiro',
    'Março',
    'Abril',
    'Maio',
    'Junho',
    'Julho',
    'Agosto',
    'Setembro',
    'Outubro',
    'Novembro',
    'Dezembro'
  ];


  // =========================================================
  // CONTROLE DE SUBSCRIPTIONS
  // =========================================================

  private readonly cancelarConsultas =
    new Subject<void>();

  private readonly destruir =
    new Subject<void>();


  // =========================================================
  // ORÇAMENTOS
  // =========================================================

  orcamentos:
    Orcamento[] = [];

  carregandoOrcamentos =
    true;

  erroOrcamentos =
    '';


  get totalPlanejado(): number {

    return this.orcamentos.reduce(
      (total, item) =>
        total + item.valorLimite,
      0
    );
  }


  get totalGasto(): number {

    return this.orcamentos.reduce(
      (total, item) =>
        total + item.valorGasto,
      0
    );
  }


  get totalDisponivel(): number {

    return this.totalPlanejado -
      this.totalGasto;
  }


  larguraProgresso(
    percentual: number
  ): number {

    return Math.min(
      Math.max(
        percentual,
        0
      ),
      100
    );
  }


  // =========================================================
  // OPÇÕES DOS GRÁFICOS
  // =========================================================

  readonly opcoesRosca:
    ChartConfiguration<'doughnut'>['options'] = {

      responsive: true,

      cutout: '72%',

      plugins: {
        legend: {
          position: 'bottom',

          labels: {
            color: '#64748b',
            padding: 20,
            usePointStyle: true,

            font: {
              family: 'Arial, sans-serif',
              size: 12
            }
          }
        }
      }
    };


  readonly opcoesHistorico:
    ChartConfiguration<'bar'>['options'] = {

      responsive: true,

      maintainAspectRatio: false,

      plugins: {
        legend: {
          position: 'bottom',

          labels: {
            color: '#64748b',
            padding: 20,
            usePointStyle: true
          }
        }
      },

      scales: {

        x: {
          grid: {
            display: false
          },

          ticks: {
            color: '#64748b'
          }
        },

        y: {
          beginAtZero: true,

          border: {
            display: false
          },

          grid: {
            color: '#edf1f5'
          },

          ticks: {
            color: '#64748b'
          }
        }
      }
    };


  // =========================================================
  // GRÁFICO - COMPOSIÇÃO DO PATRIMÔNIO
  // =========================================================

  graficoPatrimonio:
    ChartConfiguration<'doughnut'>['data'] = {

      labels: [
        'Contas',
        'Investimentos'
      ],

      datasets: [
        {
          backgroundColor: [
            '#274c77',
            '#168575',
            '#d5a451',
            '#7e8eb2',
            '#b97983',
            '#85b9c1'
          ],

          borderWidth: 3,

          borderColor:
            '#ffffff',

          hoverOffset: 5,

          data: [
            0,
            0
          ]
        }
      ]
    };


  // =========================================================
  // GRÁFICO - EVOLUÇÃO PATRIMONIAL
  // =========================================================

  graficoEvolucaoPatrimonial:
    ChartConfiguration<'line'>['data'] = {

      labels: [],

      datasets: [
        {
          label:
            'Patrimônio total',

          data: [],

          borderColor:
            '#274c77',

          backgroundColor:
            '#274c77',

          tension:
            0.3,

          fill:
            false,

          pointRadius:
            4,

          pointHoverRadius:
            6
        }
      ]
    };


  readonly opcoesEvolucaoPatrimonial:
    ChartConfiguration<'line'>['options'] = {

      responsive:
        true,

      maintainAspectRatio:
        false,

      plugins: {

        legend: {
          position:
            'bottom'
        }
      },

      scales: {

        x: {

          grid: {
            display:
              false
          }
        },

        y: {

          beginAtZero:
            false,

          border: {
            display:
              false
          }
        }
      }
    };


  // =========================================================
  // GRÁFICO - DESPESAS POR CATEGORIA
  // =========================================================

  graficoDespesas:
    ChartConfiguration<'doughnut'>['data'] = {

      labels: [],

      datasets: [
        {
          backgroundColor: [
            '#274c77',
            '#168575',
            '#d5a451',
            '#7e8eb2',
            '#b97983',
            '#85b9c1'
          ],

          borderWidth:
            3,

          borderColor:
            '#ffffff',

          hoverOffset:
            5,

          data:
            []
        }
      ]
    };


  // =========================================================
  // GRÁFICO - RECEITAS X DESPESAS POR MÊS
  // =========================================================

  graficoHistorico:
    ChartConfiguration<'bar'>['data'] = {

      labels: [],

      datasets: [

        {
          label:
            'Receitas',

          backgroundColor:
            '#168575',

          borderRadius:
            5,

          maxBarThickness:
            32,

          data:
            []
        },

        {
          label:
            'Despesas',

          backgroundColor:
            '#d58178',

          borderRadius:
            5,

          maxBarThickness:
            32,

          data:
            []
        }
      ]
    };


  // =========================================================
  // MENSAGENS DE ERRO
  // =========================================================

  erroResumo = '';

  erroCategorias = '';

  erroHistorico = '';

  erroContas = '';

  erroInvestimentos = '';

  erroPatrimonio = '';

  erroEvolucaoPatrimonial = '';


  // =========================================================
  // CONTROLE DE CARREGAMENTO
  // =========================================================

  carregandoCategorias =
    true;

  carregandoHistorico =
    true;

  carregandoContas =
    true;

  carregandoInvestimentos =
    true;

  carregandoPatrimonio =
    true;

  carregandoEvolucaoPatrimonial =
    true;


  // =========================================================
  // CONSTRUTOR
  // =========================================================

  constructor(

    private route:
      ActivatedRoute,

    private router:
      Router,

    private dashboardService:
      DashboardService,

    private contaService:
      ContaService,

    private orcamentoService:
      OrcamentoService

  ) { }


  // =========================================================
  // INICIALIZAÇÃO
  // =========================================================

  ngOnInit(): void {

    this.visao =
      this.route.snapshot.data['visao']
      ?? 'sintetico';


    this.route.queryParamMap

      .pipe(
        takeUntil(
          this.destruir
        )
      )

      .subscribe(params => {

        const hoje =
          new Date();

        const ano =
          Number(
            params.get('ano')
            ?? hoje.getFullYear()
          );

        const mes =
          Number(
            params.get('mes')
            ?? hoje.getMonth() + 1
          );


        if (
          !this.periodoValido(
            ano,
            mes
          )
        ) {

          this.erroPeriodo =
            'Período inválido no endereço. Exibindo o mês atual.';

          this.periodo = {
            ano:
              hoje.getFullYear(),

            mes:
              hoje.getMonth() + 1
          };
        }
        else {

          this.erroPeriodo =
            '';

          this.periodo = {
            ano,
            mes
          };
        }


        this.filtroAno =
          this.periodo.ano;

        this.filtroMes =
          this.periodo.mes;

        this.carregarDashboard();
      });
  }


  // =========================================================
  // DESTRUIÇÃO
  // =========================================================

  ngOnDestroy(): void {

    this.cancelarConsultas.next();

    this.cancelarConsultas.complete();

    this.destruir.next();

    this.destruir.complete();
  }


  // =========================================================
  // VALIDAÇÃO DO PERÍODO
  // =========================================================

  private periodoValido(
    ano: number,
    mes: number
  ): boolean {

    return (
      Number.isInteger(ano) &&
      ano >= 1 &&
      ano <= 9999 &&

      Number.isInteger(mes) &&
      mes >= 1 &&
      mes <= 12
    );
  }


  // =========================================================
  // ALTERAÇÃO DO PERÍODO
  // =========================================================

  verificarPeriodo(): void {

    const ano =
      Number(
        this.filtroAno
      );

    const mes =
      Number(
        this.filtroMes
      );


    if (
      !this.periodoValido(
        ano,
        mes
      )
    ) {

      this.erroPeriodo =
        'Informe um ano entre 1 e 9999 e selecione um mês válido.';

      return;
    }


    this.erroPeriodo =
      '';


    if (
      ano === this.periodo.ano &&
      mes === this.periodo.mes
    ) {

      this.carregarDashboard();

      return;
    }


    void this.router.navigate(
      [],
      {
        relativeTo:
          this.route,

        queryParams: {
          ano,
          mes
        },

        queryParamsHandling:
          'merge'
      }
    );
  }


  // =========================================================
  // CARREGAMENTO PRINCIPAL
  // =========================================================

  private carregarDashboard(): void {

    this.cancelarConsultas.next();


    // =======================================================
    // RESET DOS DADOS
    // =======================================================

    this.resumo =
      null;

    this.resumoInvestimentos =
      null;

    this.resumoPatrimonio =
      null;

    this.despesasPorCategoria =
      [];

    this.historicoMensal =
      [];

    this.evolucaoPatrimonial =
      [];

    this.orcamentos =
      [];

    this.contas =
      [];

    this.saldoTotal =
      0;


    // =======================================================
    // RESET DOS ERROS
    // =======================================================

    this.erroResumo =
      '';

    this.erroCategorias =
      '';

    this.erroHistorico =
      '';

    this.erroContas =
      '';

    this.erroInvestimentos =
      '';

    this.erroPatrimonio =
      '';

    this.erroEvolucaoPatrimonial =
      '';

    this.erroOrcamentos =
      '';


    // =======================================================
    // RESET DOS ESTADOS DE CARREGAMENTO
    // =======================================================

    this.carregandoCategorias =
      true;

    this.carregandoHistorico =
      true;

    this.carregandoContas =
      true;

    this.carregandoInvestimentos =
      true;

    this.carregandoPatrimonio =
      true;

    this.carregandoEvolucaoPatrimonial =
      true;

    this.carregandoOrcamentos =
      true;


    // =======================================================
    // RESET DOS GRÁFICOS
    // =======================================================

    this.graficoPatrimonio = {

      labels: [
        'Contas',
        'Investimentos'
      ],

      datasets: [
        {
          backgroundColor: [
            '#274c77',
            '#168575',
            '#d5a451',
            '#7e8eb2',
            '#b97983',
            '#85b9c1'
          ],

          borderWidth:
            3,

          borderColor:
            '#ffffff',

          hoverOffset:
            5,

          data: [
            0,
            0
          ]
        }
      ]
    };


    this.graficoEvolucaoPatrimonial = {

      labels:
        [],

      datasets: [
        {
          label:
            'Patrimônio total',

          data:
            [],

          borderColor:
            '#274c77',

          backgroundColor:
            '#274c77',

          tension:
            0.3,

          fill:
            false,

          pointRadius:
            4,

          pointHoverRadius:
            6
        }
      ]
    };


    this.graficoDespesas = {

      labels:
        [],

      datasets: [
        {
          backgroundColor: [
            '#274c77',
            '#168575',
            '#d5a451',
            '#7e8eb2',
            '#b97983',
            '#85b9c1'
          ],

          borderWidth:
            3,

          borderColor:
            '#ffffff',

          hoverOffset:
            5,

          data:
            []
        }
      ]
    };


    this.graficoHistorico = {

      labels:
        [],

      datasets: [

        {
          label:
            'Receitas',

          backgroundColor:
            '#168575',

          borderRadius:
            5,

          maxBarThickness:
            32,

          data:
            []
        },

        {
          label:
            'Despesas',

          backgroundColor:
            '#d58178',

          borderRadius:
            5,

          maxBarThickness:
            32,

          data:
            []
        }
      ]
    };


    const {
      ano,
      mes
    } = this.periodo;


    // =======================================================
    // 1 - ORÇAMENTOS
    // =======================================================

    if (
      this.visao === 'orcamentos'
    ) {

      this.orcamentoService

        .listar(
          ano,
          mes
        )

        .pipe(
          takeUntil(
            this.cancelarConsultas
          )
        )

        .subscribe({

          next: orcamentos => {

            this.orcamentos =
              orcamentos;

            this.carregandoOrcamentos =
              false;
          },

          error: erro => {

            console.error(
              'Erro ao carregar orçamentos:',
              erro
            );

            this.erroOrcamentos =
              'Não foi possível carregar os orçamentos.';

            this.carregandoOrcamentos =
              false;
          }
        });
    }


    // =======================================================
    // 2 - RESUMO FINANCEIRO
    // =======================================================

    if (
      this.visao === 'sintetico' ||
      this.visao === 'financeiro'
    ) {

      this.dashboardService

        .obterResumo(
          ano,
          mes
        )

        .pipe(
          takeUntil(
            this.cancelarConsultas
          )
        )

        .subscribe({

          next: response => {

            this.resumo =
              response;
          },

          error: erro => {

            console.error(
              'Erro ao carregar resumo financeiro:',
              erro
            );

            this.erroResumo =
              'Não foi possível carregar o resumo financeiro.';
          }
        });
    }


    // =======================================================
    // 3 - DESPESAS POR CATEGORIA
    // =======================================================

    if (
      this.visao === 'financeiro'
    ) {

      this.dashboardService

        .obterDespesasPorCategoria(
          ano,
          mes
        )

        .pipe(
          takeUntil(
            this.cancelarConsultas
          )
        )

        .subscribe({

          next: response => {

            this.despesasPorCategoria =
              response;

            this.graficoDespesas = {

              labels:
                response.map(
                  despesa =>
                    despesa.categoria
                ),

              datasets: [
                {
                  backgroundColor: [
                    '#274c77',
                    '#168575',
                    '#d5a451',
                    '#7e8eb2',
                    '#b97983',
                    '#85b9c1'
                  ],

                  borderWidth:
                    3,

                  borderColor:
                    '#ffffff',

                  hoverOffset:
                    5,

                  data:
                    response.map(
                      despesa =>
                        despesa.total
                    )
                }
              ]
            };

            this.carregandoCategorias =
              false;
          },

          error: erro => {

            console.error(
              'Erro ao carregar despesas por categoria:',
              erro
            );

            this.erroCategorias =
              'Não foi possível carregar as despesas por categoria.';

            this.carregandoCategorias =
              false;
          }
        });


      // =====================================================
      // 4 - HISTÓRICO MENSAL
      // =====================================================

      this.dashboardService

        .obterHistoricoMensal(
          ano
        )

        .pipe(
          takeUntil(
            this.cancelarConsultas
          )
        )

        .subscribe({

          next: response => {

            const historicoFiltrado =
              response.filter(
                item =>
                  item.mes <= mes
              );

            this.historicoMensal =
              historicoFiltrado;

            this.carregandoHistorico =
              false;

            this.graficoHistorico = {

              labels:
                historicoFiltrado.map(
                  item =>
                    item.mesNome
                ),

              datasets: [

                {
                  label:
                    'Receitas',

                  backgroundColor:
                    '#168575',

                  borderRadius:
                    5,

                  maxBarThickness:
                    32,

                  data:
                    historicoFiltrado.map(
                      item =>
                        item.totalReceitas
                    )
                },

                {
                  label:
                    'Despesas',

                  backgroundColor:
                    '#d58178',

                  borderRadius:
                    5,

                  maxBarThickness:
                    32,

                  data:
                    historicoFiltrado.map(
                      item =>
                        item.totalDespesas
                    )
                }
              ]
            };
          },

          error: erro => {

            console.error(
              'Erro ao carregar histórico mensal:',
              erro
            );

            this.erroHistorico =
              'Não foi possível carregar o histórico mensal.';

            this.carregandoHistorico =
              false;
          }
        });
    }


    // =======================================================
    // 5 - CONTAS
    // =======================================================

    if (
      this.visao !== 'orcamentos'
    ) {

      this.dashboardService

        .obterContas()

        .pipe(
          takeUntil(
            this.cancelarConsultas
          )
        )

        .subscribe({

          next: response => {

            this.contas =
              response;


            if (
              this.contas.length === 0
            ) {

              this.saldoTotal =
                0;

              this.carregandoContas =
                false;

              return;
            }


            let saldosPendentes =
              this.contas.length;


            this.contas.forEach(
              conta => {

                this.contaService

                  .obterSaldo(
                    conta.id
                  )

                  .pipe(
                    takeUntil(
                      this.cancelarConsultas
                    )
                  )

                  .subscribe({

                    next: saldo => {

                      conta.saldoAtual =
                        saldo.saldoAtual;

                      this.calcularSaldoTotal();

                      saldosPendentes--;


                      if (
                        saldosPendentes === 0
                      ) {

                        this.carregandoContas =
                          false;
                      }
                    },

                    error: erro => {

                      console.error(
                        `Erro ao carregar saldo da conta ${conta.id}:`,
                        erro
                      );

                      this.erroContas =
                        'Não foi possível carregar todos os saldos das contas.';

                      saldosPendentes--;


                      if (
                        saldosPendentes === 0
                      ) {

                        this.carregandoContas =
                          false;
                      }
                    }
                  });
              }
            );
          },

          error: erro => {

            console.error(
              'Erro ao carregar contas:',
              erro
            );

            this.erroContas =
              'Não foi possível carregar as contas.';

            this.carregandoContas =
              false;
          }
        });
    }


    // =======================================================
    // 6 - INVESTIMENTOS, PATRIMÔNIO E EVOLUÇÃO
    // =======================================================

    if (
      this.visao === 'sintetico' ||
      this.visao === 'patrimonio'
    ) {

      this.carregarInvestimentos();

      this.carregarPatrimonio();

      this.carregarEvolucaoPatrimonial();
    }
  }


  // =========================================================
  // CARREGA RESUMO DOS INVESTIMENTOS
  // =========================================================

  private carregarInvestimentos(): void {

    this.carregandoInvestimentos =
      true;

    this.erroInvestimentos =
      '';


    this.dashboardService

      .obterResumoInvestimentos()

      .pipe(
        takeUntil(
          this.cancelarConsultas
        )
      )

      .subscribe({

        next: response => {

          this.resumoInvestimentos =
            response;

          this.carregandoInvestimentos =
            false;
        },

        error: erro => {

          console.error(
            'Erro ao carregar investimentos:',
            erro
          );

          this.erroInvestimentos =
            'Não foi possível carregar os investimentos.';

          this.carregandoInvestimentos =
            false;
        }
      });
  }


  // =========================================================
  // CARREGA PATRIMÔNIO
  // =========================================================

  private carregarPatrimonio(): void {

    this.carregandoPatrimonio =
      true;

    this.erroPatrimonio =
      '';


    this.dashboardService

      .obterPatrimonio()

      .pipe(
        takeUntil(
          this.cancelarConsultas
        )
      )

      .subscribe({

        next: response => {

          this.resumoPatrimonio =
            response;

          this.atualizarGraficoPatrimonio();

          this.carregandoPatrimonio =
            false;
        },

        error: erro => {

          console.error(
            'Erro ao carregar patrimônio:',
            erro
          );

          this.erroPatrimonio =
            'Não foi possível carregar o patrimônio.';

          this.carregandoPatrimonio =
            false;
        }
      });
  }


  // =========================================================
  // CARREGA EVOLUÇÃO PATRIMONIAL
  // =========================================================

  private carregarEvolucaoPatrimonial(): void {

    this.carregandoEvolucaoPatrimonial =
      true;

    this.erroEvolucaoPatrimonial =
      '';


    const ano =
      this.periodo.ano;


    this.dashboardService

      .obterEvolucaoPatrimonial(
        ano
      )

      .pipe(
        takeUntil(
          this.cancelarConsultas
        )
      )

      .subscribe({

        next: response => {

          /*
           * O backend já limita o ano atual
           * aos meses existentes.
           *
           * Para anos anteriores, respeitamos também
           * o mês escolhido no filtro do Dashboard.
           */
          const evolucaoFiltrada =
            response.filter(
              item =>
                item.mes <= this.periodo.mes
            );


          this.evolucaoPatrimonial =
            evolucaoFiltrada;


          this.graficoEvolucaoPatrimonial = {

            labels:
              evolucaoFiltrada.map(
                item =>
                  item.mesNome
              ),

            datasets: [
              {
                label:
                  'Patrimônio total',

                data:
                  evolucaoFiltrada.map(
                    item =>
                      item.patrimonioTotal
                  ),

                borderColor:
                  '#274c77',

                backgroundColor:
                  '#274c77',

                tension:
                  0.3,

                fill:
                  false,

                pointRadius:
                  4,

                pointHoverRadius:
                  6
              }
            ]
          };


          this.carregandoEvolucaoPatrimonial =
            false;
        },

        error: erro => {

          console.error(
            'Erro ao carregar evolução patrimonial:',
            erro
          );

          this.erroEvolucaoPatrimonial =
            'Não foi possível carregar a evolução patrimonial.';

          this.carregandoEvolucaoPatrimonial =
            false;
        }
      });
  }


  // =========================================================
  // CALCULA O SALDO TOTAL DAS CONTAS
  // =========================================================

  private calcularSaldoTotal(): void {

    this.saldoTotal =
      this.contas.reduce(

        (total, conta) =>
          total +
          (conta.saldoAtual ?? 0),

        0
      );
  }


  // =========================================================
  // ATUALIZA GRÁFICO PATRIMONIAL
  // =========================================================

  private atualizarGraficoPatrimonio(): void {

    if (
      !this.resumoPatrimonio
    ) {

      return;
    }


    this.graficoPatrimonio = {

      labels: [
        'Contas',
        'Investimentos'
      ],

      datasets: [
        {
          backgroundColor: [
            '#274c77',
            '#168575',
            '#d5a451',
            '#7e8eb2',
            '#b97983',
            '#85b9c1'
          ],

          borderWidth:
            3,

          borderColor:
            '#ffffff',

          hoverOffset:
            5,

          data: [

            this.resumoPatrimonio
              .saldoContas,

            this.resumoPatrimonio
              .valorInvestimentos
          ]
        }
      ]
    };
  }
}