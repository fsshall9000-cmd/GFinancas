import { ErrorHandlerService } from '../../../core/services/error-handler.service';
import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { ActivatedRoute, Router } from '@angular/router';

import { TransacaoService }
  from '../../../core/services/transacao.service';

import { ContaService }
  from '../../../core/services/conta.service';

import { CategoriaService }
  from '../../../core/services/categoria.service';

import { Conta }
  from '../../../models/conta';

import { Categoria }
  from '../../../models/categoria';

import { TransacaoCreate }
  from '../../../models/transacao-create';


@Component({
  selector: 'app-editar-transacao',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './editar-transacao.component.html',
  styleUrl: './editar-transacao.component.css'
})
export class EditarTransacaoComponent implements OnInit {

  id = 0;

  contas: Conta[] = [];

  categorias: Categoria[] = [];

  categoriasFiltradas: Categoria[] = [];

  carregando = true;

  salvando = false;

  erro = '';


  private readonly fb = inject(FormBuilder);

  formulario = this.fb.group({

    descricao: [
      '',
      Validators.required
    ],

    valor: [
      0,
      [
        Validators.required,
        Validators.min(0.01)
      ]
    ],

    data: [
      '',
      Validators.required
    ],

    tipo: [
      1,
      Validators.required
    ],

    contaId: [
      null as number | null,
      Validators.required
    ],

    categoriaId: [
      null as number | null,
      Validators.required
    ]

  });


  constructor(
    private errorHandler: ErrorHandlerService,
    private route: ActivatedRoute,
    private router: Router,
    private transacaoService: TransacaoService,
    private contaService: ContaService,
    private categoriaService: CategoriaService
  ) {}


  ngOnInit(): void {

    this.id = Number(
      this.route.snapshot.paramMap.get('id')
    );

    this.carregarDados();

    this.formulario
      .get('tipo')
      ?.valueChanges
      .subscribe(tipo => {

        this.filtrarCategorias(tipo);

      });
  }


  carregarDados(): void {

    this.contaService
      .listar()
      .subscribe({

        next: response => {
          this.contas = response;
        },

        error: erro => {
          console.error(
            'Erro ao carregar contas:',
            erro
          );
        }

      });


    this.categoriaService
      .listar()
      .subscribe({

        next: response => {

          this.categorias = response;

          this.carregarTransacao();
        },

        error: erro => {

          console.error(
            'Erro ao carregar categorias:',
            erro
          );

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível carregar as categorias.'
            );

        }

      });
  }


  carregarTransacao(): void {

    this.transacaoService
      .buscarPorId(this.id)
      .subscribe({

        next: transacao => {

          this.filtrarCategorias(
            transacao.tipo
          );

          /*
           * input type="date" espera
           * yyyy-MM-dd.
           *
           * API:
           * 2026-09-22T10:00:00
           *
           * Angular:
           * 2026-09-22
           */
          const data =
            transacao.data.substring(0, 10);

          this.formulario.patchValue({

            descricao:
              transacao.descricao,

            valor:
              transacao.valor,

            data:
              data,

            tipo:
              transacao.tipo,

            contaId:
              transacao.contaId,

            categoriaId:
              transacao.categoriaId

          });

          this.carregando = false;
        },

        error: erro => {

          console.error(
            'Erro ao buscar transação:',
            erro
          );

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível carregar a transação.'
            );

          this.carregando = false;
        }

      });
  }


  filtrarCategorias(
    tipo: number | null | undefined
  ): void {

    if (tipo == null) {

      this.categoriasFiltradas = [];

      return;
    }

    this.categoriasFiltradas =
      this.categorias.filter(
        categoria =>
          categoria.tipo === Number(tipo)
      );
  }


  salvar(): void {

    if (this.formulario.invalid) {

      this.formulario.markAllAsTouched();

      return;
    }


    this.salvando = true;

    this.erro = '';


    const valores =
      this.formulario.getRawValue();


    const transacao: TransacaoCreate = {

      descricao:
        valores.descricao!,

      valor:
        Number(valores.valor),

      data:
        valores.data!,

      tipo:
        Number(valores.tipo),

      contaId:
        Number(valores.contaId),

      categoriaId:
        Number(valores.categoriaId)

    };


    this.transacaoService
      .atualizar(
        this.id,
        transacao
      )
      .subscribe({

        next: () => {

          this.salvando = false;

          this.router.navigate([
            '/transacoes'
          ]);
        },

        error: erro => {

          console.error(
            'Erro ao atualizar transação:',
            erro
          );

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível atualizar a transação.'
            );

          this.salvando = false;
        }

      });
  }


  cancelar(): void {

    this.router.navigate([
      '/transacoes'
    ]);
  }
}