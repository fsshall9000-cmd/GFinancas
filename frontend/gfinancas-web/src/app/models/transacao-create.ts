export interface TransacaoCreate {
  descricao: string;
  valor: number;
  data: string;
  tipo: number;
  contaId: number;
  categoriaId: number;
}