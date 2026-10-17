import { environment } from '../../../enviroments/environment';

import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

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


@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private readonly apiUrl =
    `${environment.apiUrl}/Dashboard`;


  constructor(
    private http: HttpClient
  ) { }


  obterContas(): Observable<Conta[]> {

    return this.http.get<Conta[]>(
      `${environment.apiUrl}/Conta/Listar`
    );

  }


  obterResumo(
    ano: number,
    mes: number
  ): Observable<ResumoFinanceiro> {

    return this.http.get<ResumoFinanceiro>(
      `${this.apiUrl}/resumo?ano=${ano}&mes=${mes}`
    );

  }

  obterPatrimonio(): Observable<ResumoPatrimonio> {

  return this.http.get<ResumoPatrimonio>(
    `${this.apiUrl}/Patrimonio`
  );

}


  obterResumoInvestimentos():
    Observable<ResumoInvestimentos> {

    return this.http.get<ResumoInvestimentos>(
      `${this.apiUrl}/Investimentos`
    );

  }


  obterDespesasPorCategoria(
    ano: number,
    mes: number
  ): Observable<DespesaPorCategoria[]> {

    return this.http.get<DespesaPorCategoria[]>(
      `${this.apiUrl}/despesas-por-categoria?ano=${ano}&mes=${mes}`
    );

  }


  obterHistoricoMensal(
    ano: number
  ): Observable<HistoricoMensal[]> {

    return this.http.get<HistoricoMensal[]>(
      `${this.apiUrl}/historico?ano=${ano}`
    );

  }

  obterEvolucaoPatrimonial(
    ano: number
  ): Observable<EvolucaoPatrimonial[]> {
    return this.http.get<EvolucaoPatrimonial[]>(
      `${this.apiUrl}/EvolucaoPatrimonial`,
      {
        params: {
          ano: ano.toString()
        }
      }
    );
  }

}
