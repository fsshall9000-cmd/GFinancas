import { Component, OnInit } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';

import { OrcamentoService } from '../../../core/services/orcamento.service';
import { ErrorHandlerService } from '../../../core/services/error-handler.service';

import { Orcamento } from '../../../models/orcamento';
import { OrcamentoUpdate } from '../../../models/orcamento-update';

@Component({
  selector: 'app-editar-orcamento',
  imports: [
    FormsModule,
    RouterLink,
    CurrencyPipe
  ],
  templateUrl: './editar-orcamento.component.html',
  styleUrl: './editar-orcamento.component.css'
})
export class EditarOrcamentoComponent implements OnInit {

  id = 0;

  orcamento: Orcamento | null = null;

  valorLimite = 0;

  carregando = true;
  salvando = false;
  mensagemErro = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orcamentoService: OrcamentoService,
    private errorHandler: ErrorHandlerService
  ) {}

  ngOnInit(): void {

    this.id =
      Number(
        this.route.snapshot.paramMap.get('id')
      );

    if (!this.id) {
      this.mensagemErro =
        'Orçamento inválido.';

      this.carregando = false;

      return;
    }

    this.carregar();
  }

  carregar(): void {

    this.orcamentoService
      .obterPorId(this.id)
      .subscribe({

        next: response => {

          this.orcamento = response;

          this.valorLimite =
            response.valorLimite;

          this.carregando = false;
        },

        error: erro => {

          this.mensagemErro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível carregar o orçamento.'
            );

          this.carregando = false;
        }
      });
  }

  salvar(): void {

    if (this.valorLimite <= 0) {

      this.mensagemErro =
        'O valor limite deve ser maior que zero.';

      return;
    }

    const dto: OrcamentoUpdate = {
      valorLimite: this.valorLimite
    };

    this.salvando = true;
    this.mensagemErro = '';

    this.orcamentoService
      .atualizar(
        this.id,
        dto
      )
      .subscribe({

        next: () => {

          this.router.navigate([
            '/orcamentos'
          ]);
        },

        error: erro => {

          this.mensagemErro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível atualizar o orçamento.'
            );

          this.salvando = false;
        }
      });
  }
}