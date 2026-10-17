import { ActivatedRoute, convertToParamMap, provideRouter, Router } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { routes } from '../../app.routes';
import { ContaService } from '../../core/services/conta.service';
import { OrcamentoService } from '../../core/services/orcamento.service';
import { Orcamento } from '../../models/orcamento';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';
import { BehaviorSubject, of, Subject, throwError } from 'rxjs';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { DashboardComponent } from './dashboard.component';
import { DashboardService } from '../../core/services/dashboard.service';
import { HistoricoMensal } from '../../models/historico-mensal';
import { SaldoConta } from '../../models/saldo-conta';
import { DespesaPorCategoria } from '../../models/despesa-por-categoria';

registerLocaleData(localePt, 'pt-BR');

describe('DashboardComponent', () => {
  let fixture: ComponentFixture<DashboardComponent>;
  let service: jasmine.SpyObj<DashboardService>;
  let routeData: { visao: string };
  let queryParams: BehaviorSubject<ReturnType<typeof convertToParamMap>>;

  let orcamentoService: jasmine.SpyObj<OrcamentoService>;

  beforeEach(async () => {
    routeData = { visao: 'financeiro' };
    queryParams = new BehaviorSubject(convertToParamMap({}));
    orcamentoService = jasmine.createSpyObj('OrcamentoService', ['listar']);
    orcamentoService.listar.and.returnValue(of([]));
    service = jasmine.createSpyObj('DashboardService', ['obterResumo', 'obterDespesasPorCategoria', 'obterContas', 'obterHistoricoMensal', 'obterResumoInvestimentos']);
    service.obterResumoInvestimentos.and.returnValue(of({ totalAportado: 100, totalResgatado: 0, valorAtual: 120, resultado: 20, rentabilidadePercentual: 20, quantidadeAplicacoesAtivas: 1 }));
    service.obterHistoricoMensal.and.returnValue(of([]));
    service.obterResumo.and.returnValue(of({ ano: 2026, mes: 9, totalReceitas: 100, totalDespesas: 30, resultadoMes: 70, quantidadeTransacoes: 2 }));
    service.obterDespesasPorCategoria.and.returnValue(of([{ categoriaId: 1, categoria: 'Alimentação', total: 30 }]));
    service.obterContas.and.returnValue(of([{ id: 1, nome: 'Conta principal', tipoConta: 'Corrente', saldoInicial: 200, usuarioId: 1, usuario: 'João', tipoContaId: 1 }]));
    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: { snapshot: { data: routeData }, queryParamMap: queryParams } },
        provideCharts(withDefaultRegisterables()),
        { provide: DashboardService, useValue: service },
        { provide: OrcamentoService, useValue: orcamentoService },
        { provide: ContaService, useValue: { obterSaldo: () => of({ saldoAtual: 200 }) } }
      ]
    }).compileComponents();
    fixture = TestBed.createComponent(DashboardComponent);
  });

  it('exibe o resumo e as categorias no dashboard financeiro', () => {
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent;
    expect(text).toContain('70,00');
    expect(text).toContain('Transações no mês: 2');
    expect(text).not.toContain('Conta principal');
    expect(text).toContain('Alimentação');
    const hoje = new Date();
    expect(service.obterResumo).toHaveBeenCalledWith(hoje.getFullYear(), hoje.getMonth() + 1);
  });

  it('exibe falhas sem manter carregamento ou indicar ausência de dados', () => {
    service.obterResumo.and.returnValue(throwError(() => new Error('HTTP 500')));
    service.obterDespesasPorCategoria.and.returnValue(throwError(() => new Error('HTTP 500')));
    service.obterContas.and.returnValue(throwError(() => new Error('HTTP 500')));
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent;
    expect(fixture.nativeElement.querySelectorAll('[role="alert"]').length).toBe(2);
    expect(text).not.toContain('Carregando');
    expect(text).not.toContain('Nenhuma despesa');
  });

  it('atualiza o gráfico quando a consulta de categorias retorna', () => {
    const categorias = new Subject<DespesaPorCategoria[]>();
    service.obterDespesasPorCategoria.and.returnValue(categorias);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('canvas')).toBeNull();
    categorias.next([
      { categoriaId: 1, categoria: 'Alimentação', total: 30 },
      { categoriaId: 2, categoria: 'Transporte', total: 20 }
    ]);
    fixture.detectChanges();
    expect(fixture.componentInstance.graficoDespesas.labels).toEqual(['Alimentação', 'Transporte']);
    expect(fixture.componentInstance.graficoDespesas.datasets[0].data).toEqual([30, 20]);
    expect(fixture.nativeElement.querySelector('canvas')).not.toBeNull();
  });

  it('carrega o histórico do ano atual', () => {
    const historico = [{ mes: 1, mesNome: 'janeiro', totalReceitas: 100, totalDespesas: 30, resultado: 70 }];
    service.obterHistoricoMensal.and.returnValue(of(historico));
    fixture.detectChanges();
    expect(service.obterHistoricoMensal).toHaveBeenCalledWith(new Date().getFullYear());
    expect(fixture.componentInstance.historicoMensal).toEqual(historico);
  });

  it('diferencia carregamento e lista vazia de categorias', () => {
    const categorias = new Subject<DespesaPorCategoria[]>();
    service.obterDespesasPorCategoria.and.returnValue(categorias);
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Carregando despesas');
    categorias.next([]);
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Nenhuma despesa encontrada');
  });
  it('resume os orçamentos do mês e sinaliza limites excedidos', () => {
    routeData.visao = 'orcamentos';
    orcamentoService.listar.and.returnValue(of([
      { id: 1, ano: 2026, mes: 9, categoriaId: 1, categoria: 'Mercado', valorLimite: 100, valorGasto: 150, valorDisponivel: -50, percentualUtilizado: 150 },
      { id: 2, ano: 2026, mes: 9, categoriaId: 2, categoria: 'Lazer', valorLimite: 200, valorGasto: 50, valorDisponivel: 150, percentualUtilizado: 25 }
    ]));
    fixture.detectChanges();
    const component = fixture.componentInstance;
    expect(orcamentoService.listar).toHaveBeenCalledWith(component.periodo.ano, component.periodo.mes);
    expect(component.totalPlanejado).toBe(300);
    expect(component.totalGasto).toBe(200);
    expect(component.totalDisponivel).toBe(100);
    expect(fixture.nativeElement.textContent).toContain('Limite excedido');
    expect(fixture.nativeElement.querySelector('.progress-bar').style.width).toBe('100%');
    expect(fixture.nativeElement.querySelector('a[href^="/orcamentos"]').getAttribute('href')).toContain('ano=' + component.periodo.ano);
  });

  it('diferencia carregamento, ausência de orçamentos e falha', () => {
    routeData.visao = 'orcamentos';
    const resposta = new Subject<Orcamento[]>();
    orcamentoService.listar.and.returnValue(resposta);
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Carregando orçamentos');
    resposta.next([]);
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Nenhum orçamento cadastrado');
    resposta.error(new Error('HTTP 500'));
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Não foi possível carregar os orçamentos');
    expect(fixture.nativeElement.textContent).not.toContain('Nenhum orçamento cadastrado');
    expect(fixture.nativeElement.textContent).not.toContain('Carregando orçamentos');
  });


  it('mostra indicadores sem tabelas ou gráficos na visão geral', () => {
    routeData.visao = 'sintetico';
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent;
    expect(text).toContain('Patrimônio financeiro');
    expect(text).toContain('320,00');
    expect(fixture.nativeElement.querySelector('table, canvas')).toBeNull();
    expect(fixture.nativeElement.querySelectorAll('nav[aria-label="Dashboards"] a').length).toBe(4);
    expect(service.obterDespesasPorCategoria).not.toHaveBeenCalled();
    expect(service.obterHistoricoMensal).not.toHaveBeenCalled();
    expect(orcamentoService.listar).not.toHaveBeenCalled();
  });

  it('separa contas e investimentos das despesas e orçamentos', () => {
    routeData.visao = 'patrimonio';
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent;
    expect(text).toContain('Composição do patrimônio');
    expect(text).toContain('Conta principal');
    expect(text).toContain('Resultado acumulado');
    expect(text).not.toContain('Despesas por categoria');
    expect(service.obterResumo).not.toHaveBeenCalled();
    expect(service.obterDespesasPorCategoria).not.toHaveBeenCalled();
    expect(orcamentoService.listar).not.toHaveBeenCalled();
  });

  it('consulta somente os orçamentos no dashboard de orçamentos', () => {
    routeData.visao = 'orcamentos';
    fixture.detectChanges();
    expect(orcamentoService.listar).toHaveBeenCalled();
    expect(service.obterResumo).not.toHaveBeenCalled();
    expect(service.obterContas).not.toHaveBeenCalled();
    expect(service.obterResumoInvestimentos).not.toHaveBeenCalled();
    expect(service.obterHistoricoMensal).not.toHaveBeenCalled();
  });

  it('não mostra patrimônio parcial enquanto aguarda os saldos', () => {
    routeData.visao = 'sintetico';
    const saldo = new Subject<SaldoConta>();
    spyOn(TestBed.inject(ContaService), 'obterSaldo').and.returnValue(saldo);
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Carregando patrimônio');
    saldo.next({ contaId: 1, conta: 'Conta principal', saldoInicial: 200, totalReceitas: 0, totalDespesas: 0, saldoAtual: 200 });
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('320,00');
    expect(fixture.nativeElement.textContent).not.toContain('Carregando patrimônio');
  });

  it('não apresenta patrimônio como válido quando um saldo falha', () => {
    routeData.visao = 'sintetico';
    spyOn(TestBed.inject(ContaService), 'obterSaldo').and.returnValue(throwError(() => new Error('HTTP 500')));
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Não foi possível carregar o patrimônio completo');
    expect(fixture.nativeElement.textContent).toContain('Saldo indisponível');
  });

  it('distingue carregamento e falha do histórico de ausência de dados', () => {
    const historico = new Subject<HistoricoMensal[]>();
    service.obterHistoricoMensal.and.returnValue(historico);
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Carregando histórico mensal');
    historico.error(new Error('HTTP 500'));
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Não foi possível carregar o histórico mensal');
    expect(fixture.nativeElement.textContent).not.toContain('Nenhum histórico');
  });

  it('navega entre os dashboards pelas rotas configuradas', async () => {
    fixture.destroy();
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      providers: [
        provideRouter(routes.find(route => route.path === '')!.children!.filter(route => route.path?.startsWith('dashboard'))),
        provideCharts(withDefaultRegisterables()),
        { provide: DashboardService, useValue: service },
        { provide: OrcamentoService, useValue: orcamentoService },
        { provide: ContaService, useValue: { obterSaldo: () => of({ saldoAtual: 200 }) } }
      ]
    }).compileComponents();
    const harness = await RouterTestingHarness.create();
    const financeiro = await harness.navigateByUrl('/dashboard/financeiro?ano=2024&mes=2', DashboardComponent);
    expect(financeiro.periodo).toEqual({ ano: 2024, mes: 2 });
    expect(service.obterResumo).toHaveBeenCalledWith(2024, 2);
    financeiro.filtroAno = 2023;
    financeiro.filtroMes = 6;
    harness.routeNativeElement!.querySelector<HTMLFormElement>('form')!.dispatchEvent(new Event('submit', { bubbles: true, cancelable: true }));
    await harness.fixture.whenStable();
    harness.detectChanges();
    expect(financeiro.periodo).toEqual({ ano: 2023, mes: 6 });
    expect(TestBed.inject(Router).url).toContain('ano=2023&mes=6');
    const linkOrcamentos = harness.routeNativeElement!.querySelector<HTMLAnchorElement>('nav a[href^="/dashboard/orcamentos"]')!;
    linkOrcamentos.click();
    await harness.fixture.whenStable();
    harness.detectChanges();
    expect(orcamentoService.listar).toHaveBeenCalledWith(2023, 6);
    expect(TestBed.inject(Router).url).toBe('/dashboard/orcamentos?ano=2023&mes=6');

    for (const [url, visao] of [
      ['/dashboard', 'sintetico'],
      ['/dashboard/financeiro', 'financeiro'],
      ['/dashboard/patrimonio', 'patrimonio'],
      ['/dashboard/orcamentos', 'orcamentos'],
      ['/dashboard', 'sintetico']
    ]) {
      const component = await harness.navigateByUrl(url, DashboardComponent);
      harness.detectChanges();
      expect(component.visao).toBe(visao);
      expect(harness.routeNativeElement!.querySelector('[aria-current="page"]')?.getAttribute('href')?.split('?')[0]).toBe(url);
    }
  });


  it('aplica o mês e o ano da URL e limita o histórico ao mês escolhido', () => {
    queryParams.next(convertToParamMap({ ano: '2024', mes: '2' }));
    service.obterHistoricoMensal.and.returnValue(of([
      { mes: 1, mesNome: 'janeiro', totalReceitas: 10, totalDespesas: 5, resultado: 5 },
      { mes: 2, mesNome: 'fevereiro', totalReceitas: 20, totalDespesas: 8, resultado: 12 },
      { mes: 3, mesNome: 'março', totalReceitas: 30, totalDespesas: 9, resultado: 21 }
    ]));
    fixture.detectChanges();
    expect(service.obterResumo).toHaveBeenCalledWith(2024, 2);
    expect(service.obterDespesasPorCategoria).toHaveBeenCalledWith(2024, 2);
    expect(service.obterHistoricoMensal).toHaveBeenCalledWith(2024);
    expect(fixture.componentInstance.graficoHistorico.labels).toEqual(['janeiro', 'fevereiro']);
    expect(fixture.nativeElement.textContent).toContain('Fevereiro de 2024');
  });

  it('limpa os dados e ignora respostas atrasadas ao trocar o período', () => {
    const antigas = new Subject<DespesaPorCategoria[]>();
    const novas = new Subject<DespesaPorCategoria[]>();
    service.obterDespesasPorCategoria.and.returnValues(antigas, novas);
    fixture.detectChanges();
    queryParams.next(convertToParamMap({ ano: '2022', mes: '12' }));
    fixture.detectChanges();
    expect(fixture.componentInstance.resumo).not.toBeNull();
    expect(fixture.componentInstance.despesasPorCategoria).toEqual([]);
    expect(fixture.componentInstance.carregandoCategorias).toBeTrue();
    antigas.next([{ categoriaId: 1, categoria: 'Antiga', total: 99 }]);
    expect(fixture.componentInstance.despesasPorCategoria).toEqual([]);
    novas.next([{ categoriaId: 2, categoria: 'Nova', total: 10 }]);
    expect(fixture.componentInstance.despesasPorCategoria[0].categoria).toBe('Nova');
    expect(service.obterResumo).toHaveBeenCalledWith(2022, 12);
  });

  it('rejeita ano e mês inválidos sem consultar outro período', () => {
    fixture.detectChanges();
    const navigate = spyOn(TestBed.inject(Router), 'navigate');
    for (const [ano, mes] of [[0, 1], [10000, 1], [2024, 0], [2024, 13], [2024.5, 2]]) {
      fixture.componentInstance.filtroAno = ano;
      fixture.componentInstance.filtroMes = mes;
      fixture.componentInstance.verificarPeriodo();
      expect(fixture.componentInstance.erroPeriodo).toContain('Informe um ano');
    }
    expect(navigate).not.toHaveBeenCalled();
    expect(service.obterResumo).toHaveBeenCalledTimes(1);
  });

  it('usa o mês atual e avisa quando a URL possui período inválido', () => {
    queryParams.next(convertToParamMap({ ano: 'texto', mes: '13' }));
    fixture.detectChanges();
    const hoje = new Date();
    expect(service.obterResumo).toHaveBeenCalledWith(hoje.getFullYear(), hoje.getMonth() + 1);
    expect(fixture.nativeElement.textContent).toContain('Período inválido no endereço');
  });

  it('mantém explícito que patrimônio é a posição atual ao consultar o passado', () => {
    routeData.visao = 'patrimonio';
    queryParams.next(convertToParamMap({ ano: '2020', mes: '1' }));
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('não há posição histórica disponível');
  });

});
