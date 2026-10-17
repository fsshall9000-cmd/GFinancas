import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';

import { AplicacaoFinanceiraService }
  from '../../../core/services/aplicacao-financeira.service';

import { ErrorHandlerService }
  from '../../../core/services/error-handler.service';

import { AplicacaoFinanceira }
  from '../../../models/aplicacao-financeira.model';

import { MovimentacaoAplicacao }
  from '../../../models/movimentacao-aplicacao.model';

import { TipoMovimentacaoAplicacao }
  from '../../../enums/tipo-movimentacao-aplicacao.enum';

/**
 * Exibe o resumo da aplicação financeira e seu histórico de aportes e resgates,
 * controlando os estados de carregamento e as mensagens de erro da tela.
 */
@Component({
  selector: 'app-movimentacoes-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './movimentacoes-list.component.html',
  styleUrl: './movimentacoes-list.component.css'
})
export class MovimentacoesListComponent
  implements OnInit {

  aplicacaoId!: number;

  aplicacao?: AplicacaoFinanceira;

  movimentacoes: MovimentacaoAplicacao[] = [];

  carregando = false;

  mensagemErro = '';

  TipoMovimentacaoAplicacao =
    TipoMovimentacaoAplicacao;

  /** Injeta as dependências de navegação, consulta da aplicação e tratamento de erros. */
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private aplicacaoService:
      AplicacaoFinanceiraService,
    private errorHandler:
      ErrorHandlerService
  ) { }

  /**
   * Obtém o identificador da rota e inicia a consulta dos dados.
   * Redireciona para a listagem quando a conversão resulta em zero ou NaN.
   */
  ngOnInit(): void {

    this.aplicacaoId =
      Number(
        this.route.snapshot.paramMap.get('id')
      );

    if (!this.aplicacaoId) {
      this.router.navigate(['/aplicacoes']);
      return;
    }

    this.carregarDados();
  }

  /**
   * Carrega o resumo da aplicação antes de consultar suas movimentações.
   * Em caso de falha, exibe a mensagem de erro e encerra o carregamento.
   */
  private carregarDados(): void {

    this.carregando = true;
    this.mensagemErro = '';

    this.aplicacaoService
      .obterPorId(this.aplicacaoId)
      .subscribe({

        next: (aplicacao) => {

          this.aplicacao = aplicacao;

          this.carregarMovimentacoes();

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

  /**
   * Consulta o histórico da aplicação e encerra o carregamento da tela,
   * atualizando a lista em caso de sucesso ou a mensagem em caso de erro.
   */
  private carregarMovimentacoes(): void {

    this.aplicacaoService
      .listarMovimentacoes(this.aplicacaoId)
      .subscribe({

        next: (movimentacoes) => {

          this.movimentacoes =
            movimentacoes;

          this.carregando = false;

        },

        error: (erro) => {

          this.mensagemErro =
            this.errorHandler.obterMensagem(
              erro,
              'Não foi possível carregar o histórico.'
            );

          this.carregando = false;

        }

      });
  }

  /** Identifica aportes para aplicar os rótulos e as cores correspondentes na tabela. */
  ehAporte(
    movimentacao: MovimentacaoAplicacao
  ): boolean {

    return movimentacao.tipo ===
      TipoMovimentacaoAplicacao.Aporte;
  }

  /** Identifica resgates para aplicar os rótulos e as cores correspondentes na tabela. */
  ehResgate(
    movimentacao: MovimentacaoAplicacao
  ): boolean {

    return movimentacao.tipo ===
      TipoMovimentacaoAplicacao.Resgate;
  }
}
