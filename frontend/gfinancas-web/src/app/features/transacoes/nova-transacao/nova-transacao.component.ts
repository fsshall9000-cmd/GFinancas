import { ErrorHandlerService } from '../../../core/services/error-handler.service';
import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { TransacaoService } from '../../../core/services/transacao.service';
import { ContaService } from '../../../core/services/conta.service';
import { CategoriaService } from '../../../core/services/categoria.service';

import { Conta } from '../../../models/conta';
import { Categoria } from '../../../models/categoria';
import { TransacaoCreate } from '../../../models/transacao-create';

@Component({
  selector: 'app-nova-transacao',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './nova-transacao.component.html',
  styleUrl: './nova-transacao.component.css'
})
export class NovaTransacaoComponent implements OnInit {

  contas: Conta[] = [];

  categorias: Categoria[] = [];

  categoriasFiltradas: Categoria[] = [];

  salvando = false;

  erro = '';

  private readonly fb = inject(FormBuilder);

  formulario = this.fb.group({
    descricao: ['', Validators.required],

    valor: [
      0,
      [
        Validators.required,
        Validators.min(0.01)
      ]
    ],

    data: ['', Validators.required],

    tipo: [1, Validators.required],

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
    private transacaoService: TransacaoService,
    private contaService: ContaService,
    private categoriaService: CategoriaService,
    private router: Router
  ) {}

  ngOnInit(): void {

    this.carregarContas();

    this.carregarCategorias();

    this.formulario
      .get('tipo')
      ?.valueChanges
      .subscribe(tipo => {

        this.filtrarCategorias(tipo);

        this.formulario
          .get('categoriaId')
          ?.setValue(null);
      });
  }

  carregarContas(): void {

    this.contaService.listar().subscribe({
      next: response => {
        this.contas = response;
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
      }
    });
  }

  carregarCategorias(): void {

    this.categoriaService.listar().subscribe({
      next: response => {

        this.categorias = response;

        const tipo =
          this.formulario.get('tipo')?.value;

        this.filtrarCategorias(tipo);
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

  filtrarCategorias(
    tipo: number | null | undefined
  ): void {

    if (tipo == null) {
      this.categoriasFiltradas = [];
      return;
    }

    this.categoriasFiltradas =
      this.categorias.filter(
        categoria => categoria.tipo === Number(tipo)
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
      descricao: valores.descricao!,
      valor: Number(valores.valor),
      data: valores.data!,
      tipo: Number(valores.tipo),
      contaId: Number(valores.contaId),
      categoriaId: Number(valores.categoriaId)
    };

    this.transacaoService
      .adicionar(transacao)
      .subscribe({

        next: () => {

          this.salvando = false;

          this.router.navigate([
            '/transacoes'
          ]);
        },

        error: erro => {

          console.error(
            'Erro ao cadastrar transação:',
            erro
          );

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível cadastrar a transação.'
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