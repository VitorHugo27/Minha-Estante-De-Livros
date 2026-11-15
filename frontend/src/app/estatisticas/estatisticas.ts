export interface LivroDoAno {
  titulo: string;
  paginas: number | null;
}

export interface LeiturasDoAno {
  ano: number;
  paginas: number;
  livros: LivroDoAno[];
}

export interface LivrosPorNota {
  nota: number;
  livros: number;
}

export interface LivrosPorGenero {
  genero: string;
  livros: number;
}

export interface Estatisticas {
  totalLivros: number;
  queroLer: number;
  lendo: number;
  lidos: number;
  abandonados: number;
  fisicos: number;
  virtuais: number;
  paginasLidas: number;
  mediaAvaliacao: number | null;
  mediaDiasPorLivro: number | null;
  lidosSemData: number;
  lidosPorAno: LeiturasDoAno[];
  notas: LivrosPorNota[];
  generosMaisLidos: LivrosPorGenero[];
}
