export type FormatoLivro = 'Fisico' | 'Virtual';
export type StatusLeitura = 'QueroLer' | 'Lendo' | 'Lido' | 'Abandonado';

export interface Livro {
  id: string;
  titulo: string;
  autor: string;
  formato: FormatoLivro;
  isbn: string | null;
  genero: string | null;
  totalPaginas: number | null;
  capaUrl: string | null;
  status: StatusLeitura;
  dataInicioLeitura: string | null;
  dataFimLeitura: string | null;
  avaliacao: number | null;
  anotacoes: string | null;
  dataCadastro: string;
}

export const ROTULOS_STATUS: Record<StatusLeitura, string> = {
  QueroLer: 'Quero ler',
  Lendo: 'Lendo',
  Lido: 'Lido',
  Abandonado: 'Abandonado'
};

export const ROTULOS_FORMATO: Record<FormatoLivro, string> = {
  Fisico: 'Físico',
  Virtual: 'Virtual'
};

export interface CadastrarLivro {
  titulo: string;
  autor: string;
  formato: FormatoLivro;
  isbn: string | null;
  genero: string | null;
  totalPaginas: number | null;
  capaUrl: string | null;
  status: StatusLeitura;
  dataInicioLeitura: string | null;
  dataFimLeitura: string | null;
  avaliacao: number | null;
}

export interface AtualizarLivro {
  titulo: string;
  autor: string;
  formato: FormatoLivro;
  isbn: string | null;
  genero: string | null;
  totalPaginas: number | null;
  capaUrl: string | null;
}
