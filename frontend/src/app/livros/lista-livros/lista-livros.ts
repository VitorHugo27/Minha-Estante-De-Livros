import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, computed, inject, signal, viewChild } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { CapaLivro } from '../capa-livro/capa-livro';
import { FormatoLivro, Livro, ROTULOS_FORMATO, ROTULOS_STATUS, StatusLeitura } from '../livro';
import { LivrosApi } from '../livros-api';

type Ordenacao = 'titulo' | 'autor' | 'recentes' | 'avaliacao';

const LIVROS_POR_PAGINA = 10;

@Component({
  selector: 'app-lista-livros',
  imports: [RouterLink, DatePipe, CapaLivro],
  templateUrl: './lista-livros.html',
  styleUrl: './lista-livros.css'
})
export class ListaLivros {
  private readonly api = inject(LivrosApi);
  private readonly topo = viewChild<ElementRef<HTMLElement>>('topo');

  protected readonly rotulosStatus = ROTULOS_STATUS;
  protected readonly rotulosFormato = ROTULOS_FORMATO;
  protected readonly abas: (StatusLeitura | null)[] = [null, 'QueroLer', 'Lendo', 'Lido', 'Abandonado'];
  protected readonly notas = [1, 2, 3, 4, 5];

  protected readonly livros = signal<Livro[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);
  protected readonly erroAcao = signal<string | null>(null);
  protected readonly abaSelecionada = signal<StatusLeitura | null>(null);

  protected readonly busca = signal('');
  protected readonly formatoSelecionado = signal<FormatoLivro | null>(null);
  protected readonly ordenacao = signal<Ordenacao>('titulo');
  protected readonly paginaAtual = signal(1);

  // Os livros que passam pela busca e pelo formato. As abas e as contagens partem daqui.
  private readonly livrosFiltrados = computed(() => {
    const termo = this.normalizar(this.busca());
    const formato = this.formatoSelecionado();

    return this.livros().filter(livro =>
      (formato === null || livro.formato === formato) &&
      (termo === '' ||
        this.normalizar(livro.titulo).includes(termo) ||
        this.normalizar(livro.autor).includes(termo) ||
        this.normalizar(livro.genero ?? '').includes(termo)));
  });

  protected readonly livrosDaAba = computed(() => {
    const status = this.abaSelecionada();
    const livros = status === null
      ? this.livrosFiltrados()
      : this.livrosFiltrados().filter(livro => livro.status === status);

    return this.ordenar(livros, this.ordenacao());
  });

  protected readonly totalPaginas = computed(() =>
    Math.max(1, Math.ceil(this.livrosDaAba().length / LIVROS_POR_PAGINA)));

  // A página pedida pode deixar de existir, por exemplo ao remover o último livro dela.
  protected readonly pagina = computed(() => Math.min(this.paginaAtual(), this.totalPaginas()));

  protected readonly livrosDaPagina = computed(() => {
    const inicio = (this.pagina() - 1) * LIVROS_POR_PAGINA;
    return this.livrosDaAba().slice(inicio, inicio + LIVROS_POR_PAGINA);
  });

  protected readonly faixa = computed(() => {
    const total = this.livrosDaAba().length;
    const inicio = (this.pagina() - 1) * LIVROS_POR_PAGINA + 1;
    return { inicio, fim: Math.min(inicio + LIVROS_POR_PAGINA - 1, total), total };
  });

  // A primeira página, a última e as vizinhas da atual. Um null marca um salto (…).
  protected readonly paginas = computed(() => {
    const total = this.totalPaginas();
    const atual = this.pagina();
    const numeros = [...new Set([1, atual - 1, atual, atual + 1, total])]
      .filter(numero => numero >= 1 && numero <= total)
      .sort((a, b) => a - b);

    const itens: (number | null)[] = [];
    for (const numero of numeros) {
      const anterior = itens[itens.length - 1];
      if (typeof anterior === 'number' && numero - anterior > 1) {
        itens.push(null);
      }
      itens.push(numero);
    }
    return itens;
  });

  constructor() {
    this.api.listar().subscribe({
      next: livros => {
        this.livros.set(livros);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar os livros. Confira se a Api está rodando e recarregue a página.');
        this.carregando.set(false);
      }
    });
  }

  protected rotuloDaAba(status: StatusLeitura | null): string {
    return status === null ? 'Todos' : ROTULOS_STATUS[status];
  }

  protected quantidade(status: StatusLeitura | null): number {
    return status === null
      ? this.livrosFiltrados().length
      : this.livrosFiltrados().filter(livro => livro.status === status).length;
  }

  // Toda mudança de filtro volta para a primeira página.
  protected buscar(texto: string): void {
    this.busca.set(texto);
    this.paginaAtual.set(1);
  }

  protected selecionarAba(aba: StatusLeitura | null): void {
    this.abaSelecionada.set(aba);
    this.paginaAtual.set(1);
  }

  protected selecionarFormato(valor: string): void {
    this.formatoSelecionado.set(valor === '' ? null : (valor as FormatoLivro));
    this.paginaAtual.set(1);
  }

  protected selecionarOrdenacao(valor: string): void {
    this.ordenacao.set(valor as Ordenacao);
    this.paginaAtual.set(1);
  }

  protected irParaPagina(numero: number): void {
    this.paginaAtual.set(numero);
    this.topo()?.nativeElement.scrollIntoView({ block: 'start' });
  }

  protected iniciar(livro: Livro): void {
    this.executar(this.api.iniciarLeitura(livro.id));
  }

  protected concluir(livro: Livro): void {
    this.executar(this.api.concluirLeitura(livro.id));
  }

  protected abandonar(livro: Livro): void {
    this.executar(this.api.abandonar(livro.id));
  }

  protected avaliar(livro: Livro, nota: number): void {
    this.executar(this.api.avaliar(livro.id, nota));
  }

  protected remover(livro: Livro): void {
    if (!confirm(`Remover "${livro.titulo}" da estante?`)) {
      return;
    }

    this.erroAcao.set(null);

    this.api.remover(livro.id).subscribe({
      next: () =>
        this.livros.update(livros => livros.filter(item => item.id !== livro.id)),
      error: (resposta: HttpErrorResponse) =>
        this.erroAcao.set(resposta.error?.detail ?? 'Não foi possível remover o livro.')
    });
  }

  private executar(acao: Observable<Livro>): void {
    this.erroAcao.set(null);

    acao.subscribe({
      next: atualizado =>
        this.livros.update(livros =>
          livros.map(livro => (livro.id === atualizado.id ? atualizado : livro))),
      error: (resposta: HttpErrorResponse) =>
        this.erroAcao.set(resposta.error?.detail ?? 'Não foi possível concluir a ação.')
    });
  }

  // Deixa o texto em minúsculas e sem acentos: "Anéis" vira "aneis".
  private normalizar(texto: string): string {
    return texto
      .normalize('NFD')
      .replace(/[̀-ͯ]/g, '')
      .toLowerCase()
      .trim();
  }

  private ordenar(livros: Livro[], ordenacao: Ordenacao): Livro[] {
    // O sort altera o array em que é chamado. A cópia protege o valor guardado no computed.
    const copia = [...livros];

    switch (ordenacao) {
      case 'autor':
        return copia.sort((a, b) => a.autor.localeCompare(b.autor, 'pt-BR'));
      case 'recentes':
        return copia.sort((a, b) => Date.parse(b.dataCadastro) - Date.parse(a.dataCadastro));
      case 'avaliacao':
        return copia.sort((a, b) => (b.avaliacao ?? 0) - (a.avaliacao ?? 0));
      default:
        return copia.sort((a, b) => a.titulo.localeCompare(b.titulo, 'pt-BR'));
    }
  }
}
