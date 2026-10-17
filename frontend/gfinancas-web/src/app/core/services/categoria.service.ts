import { environment } from '../../../enviroments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Categoria } from '../../models/categoria';
import { CategoriaCreate } from '../../models/categoria-create';

@Injectable({
  providedIn: 'root'
})
export class CategoriaService {

  private readonly apiUrl =
    `${environment.apiUrl}/Categoria`;

  constructor(private http: HttpClient) {}

 listar(): Observable<Categoria[]> {
  return this.http.get<Categoria[]>(
    `${this.apiUrl}/Listar`
  );
}

adicionar(categoria: CategoriaCreate): Observable<Categoria> {
  return this.http.post<Categoria>(
    `${this.apiUrl}/Adicionar`,
    categoria
  );
}

buscarPorId(id: number): Observable<Categoria> {
  return this.http.get<Categoria>(
    `${this.apiUrl}/ObterPorId/${id}`
  );
}

atualizar(
  id: number,
  categoria: CategoriaCreate
): Observable<Categoria> {

  return this.http.put<Categoria>(
    `${this.apiUrl}/Atualizar/${id}`,
    categoria
  );
}

remover(id: number): Observable<void> {
  return this.http.delete<void>(
    `${this.apiUrl}/Remover/${id}`
  );
}




}