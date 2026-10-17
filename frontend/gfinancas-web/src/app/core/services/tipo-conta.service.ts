import { environment } from '../../../enviroments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { TipoConta } from '../../models/tipo-conta';

@Injectable({
  providedIn: 'root'
})
export class TipoContaService {

  private readonly apiUrl =
    `${environment.apiUrl}/TipoConta`;

  constructor(private http: HttpClient) {}

  listar(): Observable<TipoConta[]> {
    return this.http.get<TipoConta[]>(
      `${this.apiUrl}/Listar`
    );
  }
}