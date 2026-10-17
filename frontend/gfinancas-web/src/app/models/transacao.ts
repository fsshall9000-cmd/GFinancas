export interface Transacao {
  id: number;
  descricao: string;
  valor: number;
  data: string;

  tipo: number;
  tipoDescricao: string;

  contaId: number;
  conta: string;

  categoriaId: number;
  categoria: string;
}