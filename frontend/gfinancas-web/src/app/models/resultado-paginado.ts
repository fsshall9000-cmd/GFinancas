export interface ResultadoPaginado<T> {
  itens: T[];

  paginaAtual: number;
  tamanhoPagina: number;
  totalItens: number;
  totalPaginas: number;
}