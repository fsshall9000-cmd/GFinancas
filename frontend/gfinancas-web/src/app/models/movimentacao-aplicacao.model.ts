export interface MovimentacaoAplicacao {
  id: number;
  valor: number;
  data: string;

  tipo: number;
  tipoDescricao: string;

  aplicacaoFinanceiraId: number;
  contaId: number;

  conta: string;
}

export interface CriarMovimentacaoAplicacao {
  aplicacaoFinanceiraId: number;
  contaId: number;
  valor: number;
  data: string;

  // 1 = Aporte
  // 2 = Resgate
  tipo: number;
}