import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { DashboardService } from './dashboard.service';

describe('DashboardService', () => {
  let service: DashboardService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(DashboardService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());

  it('consulta as rotas da API com o período solicitado', () => {
    service.obterResumo(2026, 9).subscribe();
    http.expectOne('http://localhost:5143/api/Dashboard/resumo?ano=2026&mes=9').flush({});
    service.obterDespesasPorCategoria(2026, 9).subscribe();
    http.expectOne('http://localhost:5143/api/Dashboard/despesas-por-categoria?ano=2026&mes=9').flush([]);
    service.obterContas().subscribe();
    http.expectOne('http://localhost:5143/api/Conta/Listar').flush([]);
  });
});
