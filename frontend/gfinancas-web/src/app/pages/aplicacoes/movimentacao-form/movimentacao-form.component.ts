import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';

import { AplicacaoFinanceiraService }
  from '../../../core/services/aplicacao-financeira.service';

import { ContaService }
  from '../../../core/services/conta.service';

import { ErrorHandlerService }
  from '../../../core/services/error-handler.service';

import { AplicacaoFinanceira }
  from '../../../models/aplicacao-financeira.model';

import { CriarMovimentacaoAplicacao }
  from '../../../models/movimentacao-aplicacao.model';

import { TipoMovimentacaoAplicacao }
  from '../../../enums/tipo-movimentacao-aplicacao.enum';

@Component({
  selector: 'app-movimentacao-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './movimentacao-form.component.html',
  styleUrl: './movimentacao-form.component.css'
})
export class MovimentacaoFormComponent
  implements OnInit {

  formulario!: FormGroup;

  aplicacao?: AplicacaoFinanceira;

  aplicacaoId!: number;

  contas: any[] = [];

  saldoConta?: number;

  carregando = false;
  salvando = false;

  mensagemErro = '';

  TipoMovimentacaoAplicacao =
    TipoMovimentacaoAplicacao;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private aplicacaoService:
      AplicacaoFinanceiraService,
    private contaService:
      ContaService,
    private errorHandler:
      ErrorHandlerService
  ) { }

  ngOnInit(): void {

    this.aplicacaoId =
      Number(
        this.route.snapshot.paramMap.get('id')
      );

    if (!this.aplicacaoId) {
      this.router.navigate(['/aplicacoes']);
      return;
    }

    this.criarFormulario();

    this.carregarAplicacao();

    this.carregarContas();

    this.observarConta();
  }

  private criarFormulario(): void {

    const hoje =
      new Date()
        .toISOString()
        .substring(0, 10);

    this.formulario =
      this.fb.group({

        tipo: [
          TipoMovimentacaoAplicacao.Aporte,
          Validators.required
        ],

        contaId: [
          null,
          Validators.required
        ],

        valor: [
          null,
          [
            Validators.required,
            Validators.min(0.01)
          ]
        ],

        data: [
          hoje,
          Validators.required
        ]

      });
  }

  private carregarAplicacao(): void {

    this.carregando = true;

    this.aplicacaoService
      .obterPorId(this.aplicacaoId)
      .subscribe({

        next: (aplicacao) => {

          this.aplicacao = aplicacao;

          this.carregando = false;
        },

        error: (erro) => {

          this.mensagemErro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível carregar a aplicação.'
            );

          this.carregando = false;
        }

      });
  }

  private carregarContas(): void {

    this.contaService
      .listar()
      .subscribe({

        next: (contas) => {

          this.contas = contas;

        },

        error: (erro) => {

          this.mensagemErro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível carregar as contas.'
            );

        }

      });
  }

  private observarConta(): void {

    this.formulario
      .get('contaId')
      ?.valueChanges
      .subscribe((contaId) => {

        if (!contaId) {
          this.saldoConta = undefined;
          return;
        }

        this.carregarSaldoConta(
          Number(contaId)
        );

      });
  }

  private carregarSaldoConta(
    contaId: number
  ): void {

    this.contaService
      .obterSaldo(contaId)
      .subscribe({

        next: (resultado: any) => {

          this.saldoConta =
            resultado.saldoAtual;

        },

        error: (erro) => {

          this.saldoConta =
            undefined;

          this.mensagemErro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível consultar o saldo da conta.'
            );

        }

      });
  }

  salvar(): void {

    this.mensagemErro = '';

    if (this.formulario.invalid) {

      this.formulario.markAllAsTouched();

      return;
    }

    const dto:
      CriarMovimentacaoAplicacao = {

        aplicacaoFinanceiraId:
          this.aplicacaoId,

        contaId:
          Number(
            this.formulario
              .get('contaId')
              ?.value
          ),

        valor:
          Number(
            this.formulario
              .get('valor')
              ?.value
          ),

        data:
          this.formulario
            .get('data')
            ?.value,

        tipo:
          Number(
            this.formulario
              .get('tipo')
              ?.value
          )

      };

    this.salvando = true;

    this.aplicacaoService
      .registrarMovimentacao(dto)
      .subscribe({

        next: () => {

          this.salvando = false;

          this.router.navigate([
            '/aplicacoes'
          ]);

        },

        error: (erro) => {

          this.mensagemErro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível realizar a movimentação.'
            );

          this.salvando = false;

        }

      });
  }

  campoInvalido(
    campo: string
  ): boolean {

    const controle =
      this.formulario.get(campo);

    return !!(
      controle &&
      controle.invalid &&
      controle.touched
    );
  }

  get ehAporte(): boolean {

    return Number(
      this.formulario
        .get('tipo')
        ?.value
    ) ===
      TipoMovimentacaoAplicacao.Aporte;
  }

  get ehResgate(): boolean {

    return Number(
      this.formulario
        .get('tipo')
        ?.value
    ) ===
      TipoMovimentacaoAplicacao.Resgate;
  }
}
