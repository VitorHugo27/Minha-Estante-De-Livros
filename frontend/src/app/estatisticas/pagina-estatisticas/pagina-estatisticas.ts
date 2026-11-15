import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { corDoTecido } from '../../livros/tecido';
import { Estatisticas } from '../estatisticas';
import { EstatisticasApi } from '../estatisticas-api';

const NUMERO = new Intl.NumberFormat('pt-BR', { maximumFractionDigits: 1 });

@Component({
  selector: 'app-pagina-estatisticas',
  imports: [RouterLink],
  templateUrl: './pagina-estatisticas.html',
  styleUrl: './pagina-estatisticas.css'
})
export class PaginaEstatisticas {
  private readonly api = inject(EstatisticasApi);

  protected readonly estatisticas = signal<Estatisticas | null>(null);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);

  // Da nota 5 para a 1, como se lê um quadro de avaliações.
  protected readonly notas = computed(() => [...(this.estatisticas()?.notas ?? [])].reverse());

  private readonly maiorQuantidade = computed(() =>
    Math.max(1, ...this.notas().map(item => item.livros)));

  constructor() {
    this.api.obter().subscribe({
      next: estatisticas => {
        this.estatisticas.set(estatisticas);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar as estatísticas. Confira se a Api está rodando e recarregue a página.');
        this.carregando.set(false);
      }
    });
  }

  protected numero(valor: number): string {
    return NUMERO.format(valor);
  }

  protected quantos(quantidade: number, singular: string, plural: string): string {
    return `${this.numero(quantidade)} ${quantidade === 1 ? singular : plural}`;
  }

  protected estrelas(nota: number): string {
    return '★'.repeat(nota) + '☆'.repeat(5 - nota);
  }

  protected larguraDaBarra(livros: number): number {
    return (livros / this.maiorQuantidade()) * 100;
  }

  // Livros mais longos ficam mais altos na prateleira.
  protected alturaDaLombada(paginas: number | null): number {
    if (paginas === null) {
      return 96;
    }
    return Math.round(Math.min(136, Math.max(76, 64 + paginas / 6)));
  }

  protected tecido(titulo: string): number {
    return corDoTecido(titulo);
  }
}
