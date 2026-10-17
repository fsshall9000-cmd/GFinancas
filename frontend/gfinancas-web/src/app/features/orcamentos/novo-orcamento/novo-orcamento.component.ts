import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { OrcamentoService } from '../../../core/services/orcamento.service';
import { CategoriaService } from '../../../core/services/categoria.service';
import { ErrorHandlerService } from '../../../core/services/error-handler.service';

import { Categoria } from '../../../models/categoria';
import { OrcamentoCreate } from '../../../models/orcamento-create';

@Component({
  selector: 'app-novo-orcamento',
  imports: [
    FormsModule,
    RouterLink
  ],
  templateUrl: './novo-orcamento.component.html',
  styleUrl: './novo-orcamento.component.css'
})
export class NovoOrcamentoComponent implements OnInit {

  categorias: Categoria[] = [];

  orcamento: OrcamentoCreate = {
    ano: new Date().getFullYear(),
    mes: new Date().getMonth() + 1,
    valorLimite: 0,
    categoriaId: 0
  };

  salvando = false;
  mensagemErro = '';

  constructor(
    private orcamentoService: OrcamentoService,
    private categoriaService: CategoriaService,
    private errorHandler: ErrorHandlerService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.carregarCategorias();

    const ano = Number(
      this.route.snapshot.queryParamMap.get('ano')
    );

    const mes = Number(
      this.route.snapshot.queryParamMap.get('mes')
    );

    if (ano >= 1 && ano <= 9999) {
      this.orcamento.ano = ano;
    }

    if (mes >= 1 && mes <= 12) {
      this.orcamento.mes = mes;
    }
  }

  carregarCategorias(): void {

    this.categoriaService.listar().subscribe({

      next: response => {

        // TipoTransacao.Despesa = 2
        this.categorias =
          response.filter(categoria =>
            categoria.tipo === 2
          );
      },

      error: erro => {

        this.mensagemErro =
          this.errorHandler.obterMensagem(
            erro,
            'Não foi possível carregar as categorias.'
          );
      }
    });
  }

  salvar(): void {

    this.mensagemErro = '';

    if (this.orcamento.categoriaId <= 0) {
      this.mensagemErro =
        'Selecione uma categoria.';
      return;
    }

    if (this.orcamento.valorLimite <= 0) {
      this.mensagemErro =
        'O valor limite deve ser maior que zero.';
      return;
    }

    if (
      this.orcamento.mes < 1 ||
      this.orcamento.mes > 12
    ) {
      this.mensagemErro =
        'O mês deve estar entre 1 e 12.';
      return;
    }

    this.salvando = true;

    this.orcamentoService
      .adicionar(this.orcamento)
      .subscribe({

        next: () => {

          this.router.navigate(
            ['/orcamentos'],
            {
              queryParams: {
                ano: this.orcamento.ano,
                mes: this.orcamento.mes
              }
            }
          );
        },

        error: erro => {

          this.mensagemErro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível cadastrar o orçamento.'
            );

          this.salvando = false;
        }
      });
  }
}
