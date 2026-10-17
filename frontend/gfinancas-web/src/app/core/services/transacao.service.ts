import { environment } from '../../../enviroments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Transacao } from '../../models/transacao';
import { TransacaoCreate } from '../../models/transacao-create';
import { ResultadoPaginado } from '../../models/resultado-paginado';

@Injectable({
  providedIn: 'root'
})
export class TransacaoService {

  private readonly apiUrl =
    `${environment.apiUrl}/Transacao`;

  constructor(private http: HttpClient) {}

  listar(
    pagina: number = 1,
    tamanhoPagina: number = 10
  ): Observable<ResultadoPaginado<Transacao>> {

    return this.http.get<ResultadoPaginado<Transacao>>(
      `${this.apiUrl}/paginado?pagina=${pagina}&tamanhoPagina=${tamanhoPagina}`
    );
  }

  adicionar(
    transacao: TransacaoCreate
  ): Observable<Transacao> {

    return this.http.post<Transacao>(
      `${this.apiUrl}/Adicionar`,
      transacao
    );
  }
  remover(id: number): Observable<void> {
  return this.http.delete<void>(
    `${this.apiUrl}/Remover/${id}`
  );
}

  buscarPorId(id: number): Observable<Transacao> {
  return this.http.get<Transacao>(
    `${this.apiUrl}/BuscarPorId/${id}`
  );
}

atualizar(
  id: number,
  transacao: TransacaoCreate
): Observable<Transacao> {

  return this.http.put<Transacao>(
    `${this.apiUrl}/Atualizar/${id}`,
    transacao
  );
}





}