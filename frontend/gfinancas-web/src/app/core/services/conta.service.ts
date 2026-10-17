import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContaCreate } from '../../models/conta-create';
import { Conta } from '../../models/conta';
import { SaldoConta } from '../../models/saldo-conta';
import { environment } from '../../../enviroments/environment';

@Injectable({
  providedIn: 'root'
})
export class ContaService {

 private readonly apiUrl =
  `${environment.apiUrl}/Conta`;

  constructor(private http: HttpClient) {}

  listar(): Observable<Conta[]> {
    return this.http.get<Conta[]>(
      `${this.apiUrl}/Listar`
    );
  }

  buscarPorId(id: number): Observable<Conta> {
    return this.http.get<Conta>(
      `${this.apiUrl}/BuscarPorId/${id}`
    );
  }

  obterSaldo(id: number): Observable<SaldoConta> {
    return this.http.get<SaldoConta>(
      `${this.apiUrl}/${id}/saldo`
    );
  }

  adicionar(conta: ContaCreate): Observable<Conta> {
  return this.http.post<Conta>(
    `${this.apiUrl}/Adicionar`,
    conta
  );
}

  atualizar(
  id: number,
  conta: ContaCreate
): Observable<Conta> {

  return this.http.put<Conta>(
    `${this.apiUrl}/Atualizar/${id}`,
    conta
  );
}

remover(id: number): Observable<void> {

  return this.http.delete<void>(
    `${this.apiUrl}/Remover/${id}`
  );
}


}