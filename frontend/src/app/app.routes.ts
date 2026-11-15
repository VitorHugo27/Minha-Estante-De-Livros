import { Routes } from '@angular/router';
import { PaginaEstatisticas } from './estatisticas/pagina-estatisticas/pagina-estatisticas';
import { FormularioLivro } from './livros/formulario-livro/formulario-livro';
import { ListaLivros } from './livros/lista-livros/lista-livros';

export const routes: Routes = [
  { path: '', component: ListaLivros },
  { path: 'livros/novo', component: FormularioLivro },
  { path: 'livros/:id/editar', component: FormularioLivro },
  { path: 'estatisticas', component: PaginaEstatisticas }
];