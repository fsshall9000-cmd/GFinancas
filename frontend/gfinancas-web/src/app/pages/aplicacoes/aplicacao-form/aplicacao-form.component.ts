import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { AplicacaoFinanceiraService }
  from '../../../core/services/aplicacao-financeira.service';

import { ErrorHandlerService }
  from '../../../core/services/error-handler.service';

import {
  CriarAplicacaoFinanceira,
  AtualizarAplicacaoFinanceira
} from '../../../models/aplicacao-financeira.model';

import { TipoAplicacao }
  from '../../../enums/tipo-aplicacao.enum';

@Component({
  selector: 'app-aplicacao-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './aplicacao-form.component.html',
  styleUrl: './aplicacao-form.component.css'
})
export class AplicacaoFormComponent implements OnInit {

  formulario!: FormGroup;

  carregando = false;
  salvando = false;

  mensagemErro = '';

  aplicacaoId?: number;

  modoEdicao = false;

  tiposAplicacao = [
    {
      valor: TipoAplicacao.TesouroDireto,
      descricao: 'Tesouro Direto'
    },
    {
      valor: TipoAplicacao.CDB,
      descricao: 'CDB'
    },
    {
      valor: TipoAplicacao.LCI,
      descricao: 'LCI'
    },
    {
      valor: TipoAplicacao.LCA,
      descricao: 'LCA'
    },
    {
      valor: TipoAplicacao.FundoInvestimento,
      descricao: 'Fundo de Investimento'
    },
    {
      valor: TipoAplicacao.Acao,
      descricao: 'Ação'
    },
    {
      valor: TipoAplicacao.ETF,
      descricao: 'ETF'
    },
    {
      valor: TipoAplicacao.Outro,
      descricao: 'Outro'
    }
  ];

  constructor(
    private fb: FormBuilder,
    @Inject(AplicacaoFinanceiraService)
    private aplicacaoService: AplicacaoFinanceiraService,
    private errorHandler: ErrorHandlerService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {

    this.criarFormulario();

    const id =
      this.route.snapshot.paramMap.get('id');

    if (id) {
      this.aplicacaoId = Number(id);
      this.modoEdicao = true;

      this.carregarAplicacao();
    }
  }

  private criarFormulario(): void {

    this.formulario = this.fb.group({

      nome: [
        '',
        [
          Validators.required,
          Validators.maxLength(150)
        ]
      ],

      tipo: [
        null,
        Validators.required
      ],

      instituicao: [
        '',
        [
          Validators.required,
          Validators.maxLength(150)
        ]
      ],

      dataInicio: [
        '',
        Validators.required
      ],

      valorAtual: [
        0,
        [
          Validators.required,
          Validators.min(0)
        ]
      ]

    });
  }

  private carregarAplicacao(): void {

    if (!this.aplicacaoId) {
      return;
    }

    this.carregando = true;
    this.mensagemErro = '';

    this.aplicacaoService
      .obterPorId(this.aplicacaoId)
      .subscribe({

        next: (aplicacao) => {

          this.formulario.patchValue({
            nome: aplicacao.nome,
            tipo: aplicacao.tipo,
            instituicao: aplicacao.instituicao,
            dataInicio:
              aplicacao.dataInicio.substring(0, 10),
            valorAtual: aplicacao.valorAtual
          });

          // Tipo e data inicial não serão
          // alterados depois do cadastro.
          this.formulario
            .get('tipo')
            ?.disable();

          this.formulario
            .get('dataInicio')
            ?.disable();

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

  salvar(): void {

    this.mensagemErro = '';

    if (this.formulario.invalid) {

      this.formulario.markAllAsTouched();

      return;
    }

    if (this.modoEdicao) {
      this.atualizar();
    }
    else {
      this.criar();
    }
  }

  private criar(): void {

    const dto: CriarAplicacaoFinanceira = {

      nome:
        this.formulario.get('nome')?.value,

      tipo:
        Number(
          this.formulario.get('tipo')?.value
        ),

      instituicao:
        this.formulario.get('instituicao')?.value,

      dataInicio:
        this.formulario.get('dataInicio')?.value

    };

    this.salvando = true;

    this.aplicacaoService
      .criar(dto)
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
              'Não foi possível cadastrar a aplicação.'
            );

          this.salvando = false;
        }

      });
  }

  private atualizar(): void {

    if (!this.aplicacaoId) {
      return;
    }

    const dto: AtualizarAplicacaoFinanceira = {

      nome:
        this.formulario.get('nome')?.value,

      instituicao:
        this.formulario.get('instituicao')?.value,

      valorAtual:
        Number(
          this.formulario.get('valorAtual')?.value
        )

    };

    this.salvando = true;

    this.aplicacaoService
      .atualizar(
        this.aplicacaoId,
        dto
      )
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
              'Não foi possível atualizar a aplicação.'
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
}
