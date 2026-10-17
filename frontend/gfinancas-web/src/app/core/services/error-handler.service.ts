import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {

  obterMensagem(
    erro: HttpErrorResponse,
    mensagemPadrao?: string
  ): string {
    const corpo: unknown = erro.error;

    if (this.eMensagem(corpo)) {
      return corpo;
    }

    if (this.eObjeto(corpo)) {
      for (const campo of ['mensagem', 'message']) {
        const mensagem = corpo[campo];
        if (this.eMensagem(mensagem)) {
          return mensagem;
        }
      }

      const erros = corpo['errors'];
      if (this.eObjeto(erros)) {
        const mensagens = Object.values(erros)
          .flatMap(valor => Array.isArray(valor) ? valor : [valor])
          .filter((valor: unknown): valor is string => this.eMensagem(valor));

        if (mensagens.length > 0) {
          return mensagens.join(' ');
        }
      }

      // O backend utiliza Problem Details para mensagens de negócio.
      const detalhe = corpo['detail'];
      if (this.eMensagem(detalhe)) {
        return detalhe;
      }
    }

    // Preserva a mensagem específica da operação quando não há mensagem da API.
    if (this.eMensagem(mensagemPadrao)) {
      return mensagemPadrao;
    }

    switch (erro.status) {
      case 0:
        return 'Não foi possível conectar com o servidor.';
      case 400:
        return 'Os dados informados são inválidos.';
      case 401:
        return 'Sua sessão não é válida ou expirou.';
      case 403:
        return 'Você não possui permissão para realizar esta operação.';
      case 404:
        return 'O recurso solicitado não foi encontrado.';
      case 409:
        return 'Não foi possível concluir a operação devido a um conflito.';
      case 500:
        return 'Ocorreu um erro interno no servidor.';
      default:
        return 'Ocorreu um erro inesperado.';
    }
  }

  private eMensagem(valor: unknown): valor is string {
    return typeof valor === 'string' && valor.trim().length > 0;
  }

  private eObjeto(valor: unknown): valor is Record<string, unknown> {
    return typeof valor === 'object' && valor !== null && !Array.isArray(valor);
  }
}
