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

import { CategoriaService }
  from '../../../core/services/categoria.service';

import { CategoriaCreate }
  from '../../../models/categoria-create';

@Component({
  selector: 'app-editar-categoria',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './editar-categoria.component.html',
  styleUrl: './editar-categoria.component.css'
})
export class EditarCategoriaComponent implements OnInit {

  categoriaId = 0;

  carregando = true;

  salvando = false;

  erro = '';

  form;

  constructor(
    private errorHandler: ErrorHandlerService,
    private fb: FormBuilder,
    private categoriaService: CategoriaService,
    private route: ActivatedRoute,
    private router: Router
  ) {

    this.form = this.fb.group({

      nome: [
        '',
        Validators.required
      ],

      tipo: [
        null as number | null,
        Validators.required
      ]

    });
  }

  ngOnInit(): void {

    this.categoriaId =
      Number(
        this.route.snapshot.paramMap.get('id')
      );

    if (!this.categoriaId) {

      this.erro = 'Categoria inválida.';

      this.carregando = false;

      return;
    }

    this.carregarCategoria();
  }

  private carregarCategoria(): void {

    this.categoriaService
      .buscarPorId(this.categoriaId)
      .subscribe({

        next: categoria => {

          this.form.patchValue({
            nome: categoria.nome,
            tipo: categoria.tipo
          });

          this.carregando = false;
        },

        error: erro => {

          console.error(
            'Erro ao carregar categoria:',
            erro
          );

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível carregar a categoria.'
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

    const categoria: CategoriaCreate = {

      nome: valores.nome!,

      tipo: valores.tipo!
    };

    this.salvando = true;

    this.erro = '';

    this.categoriaService
      .atualizar(
        this.categoriaId,
        categoria
      )
      .subscribe({

        next: () => {

          this.router.navigate([
            '/categorias'
          ]);

        },

        error: erro => {

          console.error(
            'Erro ao atualizar categoria:',
            erro
          );

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível atualizar a categoria.'
            );

          this.salvando = false;
        }

      });
  }
}