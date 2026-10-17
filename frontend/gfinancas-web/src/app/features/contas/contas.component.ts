import { ErrorHandlerService } from '../../core/services/error-handler.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';

import { ContaService }
  from '../../core/services/conta.service';

import { Conta }
  from '../../models/conta';

import { SaldoConta }
  from '../../models/saldo-conta';

import { RouterLink } from '@angular/router';

interface ContaComSaldo extends Conta {
  saldoAtual?: number;
}

@Component({
  selector: 'app-contas',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './contas.component.html',
  styleUrl: './contas.component.css'
})
export class ContasComponent implements OnInit {

  contas: ContaComSaldo[] = [];

  carregando = false;

  erro = '';

  constructor(
    private errorHandler: ErrorHandlerService,
    private contaService: ContaService
  ) {}

  ngOnInit(): void {
    this.carregarContas();
  }

  carregarContas(): void {

    this.carregando = true;
    this.erro = '';

    this.contaService.listar().subscribe({

      next: contas => {

        this.contas = contas;

        this.carregando = false;

        this.carregarSaldos();
      },

      error: erro => {

        console.error(
          'Erro ao carregar contas:',
          erro
        );

        this.erro =
          this.errorHandler.obterMensagem(
            erro,
            'Não foi possível carregar as contas.'
          );

        this.carregando = false;
      }

    });
  }

  private carregarSaldos(): void {

    for (const conta of this.contas) {

      this.contaService
        .obterSaldo(conta.id)
        .subscribe({

          next: (saldo: SaldoConta) => {

            conta.saldoAtual =
              saldo.saldoAtual;
          },

          error: erro => {

            console.error(
              `Erro ao carregar saldo da conta ${conta.id}:`,
              erro
            );
          }

        });
    }
  }

  excluirConta(conta: Conta): void {

  const confirmar = confirm(
    `Deseja realmente excluir a conta "${conta.nome}"?`
  );

  if (!confirmar) {
    return;
  }

  this.contaService
    .remover(conta.id)
    .subscribe({

      next: () => {

        this.carregarContas();
      },

      error: erro => {

        console.error(
          'Erro ao excluir conta:',
          erro
        );

        if (erro.status === 400) {

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não é possível excluir esta conta porque ela possui transações.'
            );

          return;
        }

        this.erro =
          this.errorHandler.obterMensagem(
            erro,
            'Não foi possível excluir a conta.'
          );
      }

    });
}


}