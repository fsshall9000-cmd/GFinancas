import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../enviroments/environment';

import {
  AplicacaoFinanceira,
  CriarAplicacaoFinanceira,
  AtualizarAplicacaoFinanceira
} from '../../models/aplicacao-financeira.model';

import {
  MovimentacaoAplicacao,
  CriarMovimentacaoAplicacao
} from '../../models/movimentacao-aplicacao.model';

@Injectable({
  providedIn: 'root'
})
export class AplicacaoFinanceiraService {

  private readonly aplicacaoUrl =
    `${environment.apiUrl}/AplicacaoFinanceira`;

  private readonly movimentacaoUrl =
    `${environment.apiUrl}/MovimentacaoAplicacao`;

  constructor(
    private http: HttpClient
  ) { }

  criar(
    dto: CriarAplicacaoFinanceira
  ): Observable<AplicacaoFinanceira> {

    return this.http.post<AplicacaoFinanceira>(
      `${this.aplicacaoUrl}/Adicionar`,
      dto
    );
  }

  listar(): Observable<AplicacaoFinanceira[]> {

    return this.http.get<AplicacaoFinanceira[]>(
      `${this.aplicacaoUrl}/Listar`
    );
  }

  obterPorId(
    id: number
  ): Observable<AplicacaoFinanceira> {

    return this.http.get<AplicacaoFinanceira>(
      `${this.aplicacaoUrl}/ObterPorId/${id}`
    );
  }

  atualizar(
    id: number,
    dto: AtualizarAplicacaoFinanceira
  ): Observable<AplicacaoFinanceira> {

    return this.http.put<AplicacaoFinanceira>(
      `${this.aplicacaoUrl}/Atualizar/${id}`,
      dto
    );
  }

  desativar(
    id: number
  ): Observable<void> {

    return this.http.patch<void>(
      `${this.aplicacaoUrl}/Desativar/${id}`,
      {}
    );
  }

  registrarMovimentacao(
    dto: CriarMovimentacaoAplicacao
  ): Observable<MovimentacaoAplicacao> {

    return this.http.post<MovimentacaoAplicacao>(
      `${this.movimentacaoUrl}/Registrar`,
      dto
    );
  }

  listarMovimentacoes(
    aplicacaoId: number
  ): Observable<MovimentacaoAplicacao[]> {

    return this.http.get<MovimentacaoAplicacao[]>(
      `${this.movimentacaoUrl}/ListarPorAplicacao/${aplicacaoId}`
    );
  }
}
