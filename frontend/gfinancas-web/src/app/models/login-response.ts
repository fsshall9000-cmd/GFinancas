export interface LoginResponse {
  token: string;
  expiracao: string;
  usuarioId: number;
  nome: string;
  email: string;
}