import { ErrorHandlerService } from '../../../core/services/error-handler.service';
import { Component, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';

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
  selector: 'app-editar-conta',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './editar-conta.component.html',
  styleUrl: './editar-conta.component.css'
})
export class EditarContaComponent implements OnInit {

  contaId = 0;

  tiposConta: TipoConta[] = [];

  carregando = true;

  salvando = false;

  erro = '';

  form;

  constructor(
    private errorHandler: ErrorHandlerService,
    private fb: FormBuilder,
    private contaService: ContaService,
    private tipoContaService: TipoContaService,
    private route: ActivatedRoute,
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

    this.contaId =
      Number(this.route.snapshot.paramMap.get('id'));

    if (!this.contaId) {
      this.erro = 'Conta inválida.';
      this.carregando = false;
      return;
    }

    this.carregarDados();
  }

  private carregarDados(): void {

    this.tipoContaService.listar().subscribe({

      next: tipos => {

        this.tiposConta = tipos;

        this.carregarConta();
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

        this.carregando = false;
      }

    });
  }

  private carregarConta(): void {

    this.contaService
      .buscarPorId(this.contaId)
      .subscribe({

        next: conta => {

          this.form.patchValue({
            nome: conta.nome,
            saldoInicial: conta.saldoInicial,
            tipoContaId: conta.tipoContaId
          });

          this.carregando = false;
        },

        error: erro => {

          console.error(
            'Erro ao carregar conta:',
            erro
          );

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível carregar a conta.'
            );

          this.carregando = false;
        }

      });
  }

  salvar(): void {

    if (this.form.invalid) {

      this.form.markAllAsTouched();

      return;
    }

    const valores =
      this.form.getRawValue();

    const conta: ContaCreate = {

      nome: valores.nome!,

      saldoInicial:
        valores.saldoInicial!,

      tipoContaId:
        valores.tipoContaId!
    };

    this.salvando = true;
    this.erro = '';

    this.contaService
      .atualizar(this.contaId, conta)
      .subscribe({

        next: () => {

          this.router.navigate([
            '/contas'
          ]);
        },

        error: erro => {

          console.error(
            'Erro ao atualizar conta:',
            erro
          );

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível atualizar a conta.'
            );

          this.salvando = false;
        }

      });
  }
}