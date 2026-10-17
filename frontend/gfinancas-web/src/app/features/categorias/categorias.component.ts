import { ErrorHandlerService } from '../../core/services/error-handler.service';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CategoriaService }
  from '../../core/services/categoria.service';

import { Categoria }
  from '../../models/categoria';

@Component({
  selector: 'app-categorias',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './categorias.component.html',
  styleUrl: './categorias.component.css'
})
export class CategoriasComponent implements OnInit {

  categorias: Categoria[] = [];
  carregando = false;
  erro = '';

  constructor(
    private errorHandler: ErrorHandlerService,
    private categoriaService: CategoriaService
  ) {}

  ngOnInit(): void {
    this.carregarCategorias();
  }

  carregarCategorias(): void {
    this.carregando = true;
    this.erro = '';

    this.categoriaService.listar().subscribe({
      next: categorias => {
        this.categorias = categorias;
        this.carregando = false;
      },

      error: erro => {
        console.error('Erro ao carregar categorias:', erro);
        this.erro = this.errorHandler.obterMensagem(
            erro,
            'Não foi possível carregar as categorias.'
          );
        this.carregando = false;
      }

      
      
    });
  }

  excluirCategoria(
  categoria: Categoria
): void {

  const confirmar = confirm(
    `Deseja realmente excluir a categoria "${categoria.nome}"?`
  );

  if (!confirmar) {
    return;
  }

  this.categoriaService
    .remover(categoria.id)
    .subscribe({

      next: () => {

        this.carregarCategorias();

      },

      error: erro => {

        console.error(
          'Erro ao excluir categoria:',
          erro
        );

        if (erro.status === 400) {

          this.erro =
            this.errorHandler.obterMensagem(
              erro,
              'Não é possível excluir esta categoria porque ela possui transações.'
            );

          return;
        }

        this.erro =
          this.errorHandler.obterMensagem(
            erro,
            'Não foi possível excluir a categoria.'
          );
      }

    });
}
}