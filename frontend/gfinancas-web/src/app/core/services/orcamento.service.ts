import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../enviroments/environment';
import { Orcamento } from '../../models/orcamento';
import { OrcamentoCreate } from '../../models/orcamento-create';
import { OrcamentoUpdate } from '../../models/orcamento-update';

@Injectable({
  providedIn: 'root'
})
export class OrcamentoService {

  private readonly apiUrl =
    `${environment.apiUrl}/Orcamento`;

  constructor(private http: HttpClient) {}

  adicionar(
    orcamento: OrcamentoCreate
  ): Observable<Orcamento> {

    return this.http.post<Orcamento>(
      `${this.apiUrl}/Adicionar`,
      orcamento
    );
  }

  listar(
    ano: number,
    mes: number
  ): Observable<Orcamento[]> {

    return this.http.get<Orcamento[]>(
      `${this.apiUrl}/Listar`,
      {
        params: {
          ano: ano.toString(),
          mes: mes.toString()
        }
      }
    );
  }

  obterPorId(
    id: number
  ): Observable<Orcamento> {

    return this.http.get<Orcamento>(
      `${this.apiUrl}/ObterPorId/${id}`
    );
  }

  atualizar(
    id: number,
    orcamento: OrcamentoUpdate
  ): Observable<Orcamento> {

    return this.http.put<Orcamento>(
      `${this.apiUrl}/Atualizar/${id}`,
      orcamento
    );
  }

  remover(
    id: number
  ): Observable<void> {

    return this.http.delete<void>(
      `${this.apiUrl}/Remover/${id}`
    );
  }
}
