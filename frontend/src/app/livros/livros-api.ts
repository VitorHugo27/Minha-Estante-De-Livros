import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AtualizarLivro, CadastrarLivro, Livro } from './livro';

@Injectable({ providedIn: 'root' })
export class LivrosApi {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/livros`;

  listar(): Observable<Livro[]> {
    return this.http.get<Livro[]>(this.url);
  }

  obter(id: string): Observable<Livro> {
    return this.http.get<Livro>(`${this.url}/${id}`);
  }

  cadastrar(livro: CadastrarLivro): Observable<Livro> {
    return this.http.post<Livro>(this.url, livro);
  }

  atualizarDados(id: string, dados: AtualizarLivro): Observable<Livro> {
    return this.http.put<Livro>(`${this.url}/${id}`, dados);
  }

  atualizarAnotacoes(id: string, anotacoes: string | null): Observable<Livro> {
    return this.http.put<Livro>(`${this.url}/${id}/anotacoes`, { anotacoes });
  }

  remover(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }

  iniciarLeitura(id: string, data: string | null = null): Observable<Livro> {
    return this.http.post<Livro>(`${this.url}/${id}/iniciar-leitura`, { data });
  }

  concluirLeitura(id: string, data: string | null = null): Observable<Livro> {
    return this.http.post<Livro>(`${this.url}/${id}/concluir-leitura`, { data });
  }

  abandonar(id: string): Observable<Livro> {
    return this.http.post<Livro>(`${this.url}/${id}/abandonar`, null);
  }

  avaliar(id: string, nota: number): Observable<Livro> {
    return this.http.put<Livro>(`${this.url}/${id}/avaliacao`, { nota });
  }
}
