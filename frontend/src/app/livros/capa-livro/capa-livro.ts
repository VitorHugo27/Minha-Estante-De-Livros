import { Component, computed, input, signal } from '@angular/core';
import { corDoTecido } from '../tecido';

// Mostra a capa do livro. Tenta, nesta ordem: o link informado, a capa da Open Library
// pelo ISBN e, se nenhuma imagem carregar, uma capa tipográfica com o título e o autor.
@Component({
  selector: 'app-capa-livro',
  templateUrl: './capa-livro.html',
  styleUrl: './capa-livro.css'
})
export class CapaLivro {
  readonly titulo = input.required<string>();
  readonly autor = input('');
  readonly capaUrl = input<string | null>(null);
  readonly isbn = input<string | null>(null);

  // Endereços que já falharam nesta capa. Um endereço novo é sempre tentado.
  private readonly falhas = signal<ReadonlySet<string>>(new Set());

  private readonly fontes = computed(() => {
    const fontes: string[] = [];

    const link = this.capaUrl();
    if (link) {
      fontes.push(link);
    }

    const isbn = (this.isbn() ?? '').replace(/[^0-9X]/gi, '').toUpperCase();
    if (isbn.length === 10 || isbn.length === 13) {
      // Com default=false, a Open Library responde 404 quando não tem a capa.
      fontes.push(`https://covers.openlibrary.org/b/isbn/${isbn}-M.jpg?default=false`);
    }

    return fontes;
  });

  protected readonly imagem = computed(() =>
    this.fontes().find(fonte => !this.falhas().has(fonte)) ?? null);

  protected readonly tecido = computed(() => corDoTecido(this.titulo()));

  protected marcarErro(fonte: string): void {
    this.falhas.update(falhas => new Set(falhas).add(fonte));
  }
}
