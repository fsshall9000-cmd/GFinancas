import { ErrorHandlerService } from '../../../core/services/error-handler.service';
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { ContaService }
  from '../../../core/services/conta.service';

import { TipoContaService }
  from '../../../core/services/tipo-conta.service';

import { TipoConta }
  from '../../../models/tipo-conta';

import { ContaCreate }
  from '../../../models/conta-create';

@Component({
  selector: 'app-nova-conta',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './nova-conta.component.html',
  styleUrl: './nova-conta.component.css'
})
export class NovaContaComponent implements OnInit {

  tiposConta: TipoConta[] = [];

  salvando = false;

  erro = '';

  form;

  constructor(
    private errorHandler: ErrorHandlerService,
    private fb: FormBuilder,
    private contaService: ContaService,
    private tipoContaService: TipoContaService,
    private router: Router
  ) {
    this.form = this.fb.group({
      nome: [
        '',
        Validators.required
      ],

      saldoInicial: [
        0,
        Validators.required
      ],

      tipoContaId: [
        null as number | null,
        Validators.required
      ]
    });
  }

  ngOnInit(): void {
    this.carregarTiposConta();
  }

  carregarTiposConta(): void {

    this.tipoContaService.listar().subscribe({

      next: tipos => {
        this.tiposConta = tipos;
      },

      error: erro => {

        console.error(
          'Erro ao carregar tipos de conta:',
          erro
        );

        this.erro =
          this.errorHandler.obterMensagem(
            erro,
            'Não foi possível carregar os tipos de conta.'
          );
      }

    });
  }

  salvar(): void {

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const valores = this.form.getRawValue();

    const conta: ContaCreate = {
      nome: valores.nome!,
      saldoInicial: valores.saldoInicial!,
      tipoContaId: valores.tipoContaId!
    };

    this.salvando = true;
    this.erro = '';

    this.contaService.adicionar(conta)
      .subscribe({

        next: () => {
          this.router.navigate(['/contas']);
        },

        error: erro => {

          console.error(
            'Erro ao cadastrar conta:',
            erro
          );

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível cadastrar a conta.'
            );

          this.salvando = false;
        }

      });
  }
}