import { ErrorHandlerService } from '../../../core/services/error-handler.service';
import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

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
  selector: 'app-nova-categoria',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './nova-categoria.component.html',
  styleUrl: './nova-categoria.component.css'
})
export class NovaCategoriaComponent {

  salvando = false;

  erro = '';

  form;

  constructor(
    private errorHandler: ErrorHandlerService,
    private fb: FormBuilder,
    private categoriaService: CategoriaService,
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

  salvar(): void {

    if (this.form.invalid) {

      this.form.markAllAsTouched();

      return;
    }

    const valores = this.form.getRawValue();

    const categoria: CategoriaCreate = {
      nome: valores.nome!,
      tipo: valores.tipo!
    };

    this.salvando = true;
    this.erro = '';

    this.categoriaService
      .adicionar(categoria)
      .subscribe({

        next: () => {

          this.router.navigate([
            '/categorias'
          ]);

        },

        error: erro => {

          console.error(
            'Erro ao cadastrar categoria:',
            erro
          );

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível cadastrar a categoria.'
            );

          this.salvando = false;
        }

      });
  }
}