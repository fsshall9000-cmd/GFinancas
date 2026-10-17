export interface Orcamento {
  id: number;
  ano: number;
  mes: number;
  categoriaId: number;
  categoria: string;
  valorLimite: number;
  valorGasto: number;
  valorDisponivel: number;
  percentualUtilizado: number;
}
