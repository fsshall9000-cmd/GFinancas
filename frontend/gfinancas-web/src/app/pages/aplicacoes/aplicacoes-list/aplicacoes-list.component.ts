import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { AplicacaoFinanceiraService }
  from '../../../core/services/aplicacao-financeira.service';

import { AplicacaoFinanceira }
  from '../../../models/aplicacao-financeira.model';

import { ErrorHandlerService }
  from '../../../core/services/error-handler.service';

@Component({
  selector: 'app-aplicacoes-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './aplicacoes-list.component.html',
  styleUrl: './aplicacoes-list.component.css'
})
export class AplicacoesListComponent implements OnInit {

  aplicacoes: AplicacaoFinanceira[] = [];

  carregando = false;

  mensagemErro = '';

  constructor(
    private aplicacaoService: AplicacaoFinanceiraService,
    private errorHandler: ErrorHandlerService
  ) { }

  ngOnInit(): void {
    this.carregarAplicacoes();
  }

  carregarAplicacoes(): void {

    this.carregando = true;
    this.mensagemErro = '';

    this.aplicacaoService
      .listar()
      .subscribe({

        next: (dados) => {
          this.aplicacoes = dados;
          this.carregando = false;
        },

        error: (erro) => {
          this.mensagemErro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível carregar as aplicações.'
            );

          this.carregando = false;
        }

      });
  }

  desativar(aplicacao: AplicacaoFinanceira): void {

    const confirmou = confirm(
      `Deseja desativar a aplicação "${aplicacao.nome}"?`
    );

    if (!confirmou) {
      return;
    }

    this.aplicacaoService
      .desativar(aplicacao.id)
      .subscribe({

        next: () => {
          this.carregarAplicacoes();
        },

        error: (erro) => {
          this.mensagemErro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível desativar a aplicação.'
            );
        }

      });
  }
}
