import { Component, OnInit } from '@angular/core';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { OrcamentoService } from '../../core/services/orcamento.service';
import { ErrorHandlerService } from '../../core/services/error-handler.service';
import { Orcamento } from '../../models/orcamento';

@Component({
  selector: 'app-orcamentos',
  imports: [
    CurrencyPipe,
    DecimalPipe,
    RouterLink,
    FormsModule

  ],
  templateUrl: './orcamentos.component.html',
  styleUrl: './orcamentos.component.css'
})
export class OrcamentosComponent implements OnInit {

  orcamentos: Orcamento[] = [];

  ano = new Date().getFullYear();
  mes = new Date().getMonth() + 1;

  meses = [
  { valor: 1, nome: 'Janeiro' },
  { valor: 2, nome: 'Fevereiro' },
  { valor: 3, nome: 'Março' },
  { valor: 4, nome: 'Abril' },
  { valor: 5, nome: 'Maio' },
  { valor: 6, nome: 'Junho' },
  { valor: 7, nome: 'Julho' },
  { valor: 8, nome: 'Agosto' },
  { valor: 9, nome: 'Setembro' },
  { valor: 10, nome: 'Outubro' },
  { valor: 11, nome: 'Novembro' },
  { valor: 12, nome: 'Dezembro' }
];

  carregando = false;
  mensagemErro = '';

  constructor(
    private orcamentoService: OrcamentoService,
    private errorHandler: ErrorHandlerService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
     const ano =
    Number(
      this.route.snapshot.queryParamMap.get('ano')
    );

  const mes =
    Number(
      this.route.snapshot.queryParamMap.get('mes')
    );

  if (ano >= 1 && ano <= 9999) {
    this.ano = ano;
  }

  if (mes >= 1 && mes <= 12) {
    this.mes = mes;
  }

    this.carregar();
  }
  filtrar(): void {

  if (this.mes < 1 || this.mes > 12) {
    this.mensagemErro =
      'Selecione um mês válido.';
    return;
  }

  if (this.ano < 1 || this.ano > 9999) {
    this.mensagemErro =
      'Informe um ano válido.';
    return;
  }

  this.carregar();
}
  carregar(): void {
    this.carregando = true;
    this.mensagemErro = '';

    this.orcamentoService
      .listar(this.ano, this.mes)
      .subscribe({
        next: response => {
          this.orcamentos = response;
          this.carregando = false;
        },

        error: erro => {
          this.mensagemErro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível carregar os orçamentos.'
            );

          this.carregando = false;
        }
      });
  }

  remover(id: number): void {

    const confirmar = confirm(
      'Deseja realmente excluir este orçamento?'
    );

    if (!confirmar) {
      return;
    }

    this.orcamentoService
      .remover(id)
      .subscribe({
        next: () => {
          this.carregar();
        },

        error: erro => {
          this.mensagemErro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível excluir o orçamento.'
            );
        }
      });
  }

  get totalPlanejado(): number {

    return this.orcamentos.reduce(
      (total, item) =>
        total + item.valorLimite,
      0
    );
  }

  get totalGasto(): number {

    return this.orcamentos.reduce(
      (total, item) =>
        total + item.valorGasto,
      0
    );
  }

  get totalDisponivel(): number {

    return this.totalPlanejado -
           this.totalGasto;
  }

  larguraProgresso(
    percentual: number
  ): number {

    return Math.min(
      Math.max(percentual, 0),
      100
    );
  }
}