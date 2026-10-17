import { ErrorHandlerService } from '../../core/services/error-handler.service';
import { Component, OnInit } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TransacaoService } from '../../core/services/transacao.service';
import { Transacao } from '../../models/transacao';

@Component({
  selector: 'app-transacoes',

  imports: [
    CurrencyPipe,
    DatePipe,
    RouterLink
  ],

  templateUrl: './transacoes.component.html',
  styleUrl: './transacoes.component.css'
})
export class TransacoesComponent implements OnInit {

  transacoes: Transacao[] = [];

  paginaAtual = 1;

  tamanhoPagina = 10;

  totalItens = 0;

  totalPaginas = 0;

  carregando = false;

  erro = '';

  constructor(
    private errorHandler: ErrorHandlerService,
    private transacaoService: TransacaoService
  ) {}

  ngOnInit(): void {
    this.carregarTransacoes();
  }

  carregarTransacoes(): void {

    this.carregando = true;
    this.erro = '';

    this.transacaoService
      .listar(
        this.paginaAtual,
        this.tamanhoPagina
      )
      .subscribe({

        next: response => {

          this.transacoes = response.itens;

          this.paginaAtual =
            response.paginaAtual;

          this.tamanhoPagina =
            response.tamanhoPagina;

          this.totalItens =
            response.totalItens;

          this.totalPaginas =
            response.totalPaginas;

          this.carregando = false;

          console.log(
            'Transações:',
            response
          );
        },

        error: erro => {

          console.error(
            'Erro ao carregar transações:',
            erro
          );

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível carregar as transações.'
            );

          this.carregando = false;
        }

      });
  }

  paginaAnterior(): void {
    if (this.paginaAtual > 1) {
      this.paginaAtual--;
      this.carregarTransacoes();
    }
  }

  proximaPagina(): void {
    if (this.paginaAtual < this.totalPaginas) {
      this.paginaAtual++;
      this.carregarTransacoes();
    }
  }

  excluirTransacao(transacao: Transacao): void {

  const confirmar = confirm(
    `Deseja realmente excluir a transação "${transacao.descricao}"?`
  );

  if (!confirmar) {
    return;
  }

  this.transacaoService
    .remover(transacao.id)
    .subscribe({

      next: () => {

        // Se excluímos o único registro da página atual
        // e não estamos na primeira página,
        // voltamos uma página.
        if (
          this.transacoes.length === 1 &&
          this.paginaAtual > 1
        ) {
          this.paginaAtual--;
        }

        this.carregarTransacoes();
      },

      error: erro => {

        console.error(
          'Erro ao excluir transação:',
          erro
        );

        this.erro =
          this.errorHandler.obterMensagem(
            erro,
            'Não foi possível excluir a transação.'
          );
      }
    });
}
}
