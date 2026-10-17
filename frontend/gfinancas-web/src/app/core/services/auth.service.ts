import { environment } from '../../../enviroments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Login } from '../../models/login';
import { Usuario } from '../../models/usuario';
import { LoginResponse } from '../../models/login-response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly apiUrl = `${environment.apiUrl}/Auth`;

  constructor(private http: HttpClient) { }

  login(dados: Login): Observable<LoginResponse> {

    return this.http.post<LoginResponse>(
      `${this.apiUrl}/login`,
      dados
    );
  }
  cadastrar(dados: { nome: string; email: string; senha: string }): Observable<Usuario> {
    return this.http.post<Usuario>(`${environment.apiUrl}/Usuario/Adicionar`, dados);
  }

  recuperarSenha(email: string): Observable<{ mensagem: string }> {
    return this.http.post<{ mensagem: string }>(`${this.apiUrl}/esqueceu-senha`, { email });
  }

  redefinirSenha(token: string, senha: string): Observable<{ mensagem: string }> {
    return this.http.post<{ mensagem: string }>(`${this.apiUrl}/redefinir-senha`, { token, senha });
  }

  obterUsuarioAtual(): Observable<Usuario> {
  return this.http.get<Usuario>(
    `${environment.apiUrl}/Usuario/me`
  );
  }
    


  salvarToken(token: string): void {
  localStorage.setItem('token', token);
  }

  obterToken(): string | null {
  return localStorage.getItem('token');
  }

  removerToken(): void {
  localStorage.removeItem('token');
  }
}
