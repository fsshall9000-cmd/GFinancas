export interface AplicacaoFinanceira {
  id: number;
  nome: string;
  tipo: number;
  tipoDescricao: string;
  instituicao: string;
  dataInicio: string;

  totalAportado: number;
  totalResgatado: number;
  capitalLiquido: number;

  valorAtual: number;
  resultado: number;
  rentabilidadePercentual: number;

  ativa: boolean;
}

export interface CriarAplicacaoFinanceira {
  nome: string;
  tipo: number;
  instituicao: string;
  dataInicio: string;
}

export interface AtualizarAplicacaoFinanceira {
  nome: string;
  instituicao: string;
  valorAtual: number;
}