import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable, switchMap } from 'rxjs';
import { CapaLivro } from '../capa-livro/capa-livro';
import {
  AtualizarLivro,
  CadastrarLivro,
  FormatoLivro,
  Livro,
  ROTULOS_FORMATO,
  ROTULOS_STATUS,
  StatusLeitura
} from '../livro';
import { LivrosApi } from '../livros-api';

// Um endereço completo da web, como o domínio exige: http:// ou https://.
const LINK_DA_WEB = /^https?:\/\/\S+$/i;

@Component({
  selector: 'app-formulario-livro',
  imports: [ReactiveFormsModule, RouterLink, CapaLivro],
  templateUrl: './formulario-livro.html',
  styleUrl: './formulario-livro.css'
})
export class FormularioLivro {
  private readonly api = inject(LivrosApi);
  private readonly router = inject(Router);
  private readonly fb = inject(NonNullableFormBuilder);

  // Em /livros/:id/editar, o id vem da rota. Em /livros/novo, ele é null.
  private readonly id = inject(ActivatedRoute).snapshot.paramMap.get('id');
  protected readonly editando = this.id !== null;

  protected readonly rotulosFormato = ROTULOS_FORMATO;
  protected readonly rotulosStatus = ROTULOS_STATUS;
  protected readonly statusIniciais: StatusLeitura[] = ['QueroLer', 'Lendo', 'Lido'];
  protected readonly notas = [1, 2, 3, 4, 5];

  protected readonly form = this.fb.group({
    titulo: ['', [Validators.required, Validators.maxLength(200)]],
    autor: ['', [Validators.required, Validators.maxLength(150)]],
    formato: ['Fisico' as FormatoLivro],
    genero: ['', Validators.maxLength(50)],
    isbn: ['', Validators.maxLength(20)],
    totalPaginas: [null as number | null, Validators.min(1)],
    capaUrl: ['', [Validators.maxLength(1000), Validators.pattern(LINK_DA_WEB)]],
    status: ['QueroLer' as StatusLeitura],
    dataInicioLeitura: [''],
    dataFimLeitura: [''],
    avaliacao: [null as number | null],
    anotacoes: ['', Validators.maxLength(4000)]
  });

  protected readonly status = toSignal(this.form.controls.status.valueChanges, {
    initialValue: this.form.controls.status.value
  });

  // A prévia acompanha o que é digitado no título, no autor, no ISBN e no link da capa.
  private readonly tituloDigitado = toSignal(this.form.controls.titulo.valueChanges, { initialValue: '' });
  private readonly autorDigitado = toSignal(this.form.controls.autor.valueChanges, { initialValue: '' });
  private readonly linkDigitado = toSignal(this.form.controls.capaUrl.valueChanges, { initialValue: '' });
  private readonly isbnDigitado = toSignal(this.form.controls.isbn.valueChanges, { initialValue: '' });

  protected readonly previa = computed(() => {
    const link = this.linkDigitado().trim();
    return {
      titulo: this.tituloDigitado().trim(),
      autor: this.autorDigitado().trim(),
      capaUrl: LINK_DA_WEB.test(link) ? link : null,
      isbn: this.isbnDigitado().trim()
    };
  });

  protected readonly carregando = signal(this.editando);
  protected readonly salvando = signal(false);
  protected readonly erro = signal<string | null>(null);

  constructor() {
    if (this.id !== null) {
      this.api.obter(this.id).subscribe({
        next: livro => {
          this.form.patchValue({
            titulo: livro.titulo,
            autor: livro.autor,
            formato: livro.formato,
            genero: livro.genero ?? '',
            isbn: livro.isbn ?? '',
            totalPaginas: livro.totalPaginas,
            capaUrl: livro.capaUrl ?? '',
            anotacoes: livro.anotacoes ?? ''
          });
          this.carregando.set(false);
        },
        error: () => {
          this.erro.set('Não foi possível carregar o livro. Volte para a estante e tente de novo.');
          this.carregando.set(false);
        }
      });
    }
  }

  protected salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.salvando.set(true);
    this.erro.set(null);

    const requisicao = this.id === null ? this.cadastrar() : this.atualizar(this.id);

    requisicao.subscribe({
      next: () => this.router.navigate(['/']),
      error: (resposta: HttpErrorResponse) => {
        this.erro.set(resposta.error?.detail ?? 'Não foi possível salvar o livro.');
        this.salvando.set(false);
      }
    });
  }

  private cadastrar(): Observable<Livro> {
    const valores = this.form.getRawValue();
    const iniciado = valores.status !== 'QueroLer';
    const lido = valores.status === 'Lido';

    const livro: CadastrarLivro = {
      titulo: valores.titulo,
      autor: valores.autor,
      formato: valores.formato,
      isbn: valores.isbn || null,
      genero: valores.genero || null,
      totalPaginas: valores.totalPaginas,
      capaUrl: valores.capaUrl.trim() || null,
      status: valores.status,
      dataInicioLeitura: iniciado ? valores.dataInicioLeitura || null : null,
      dataFimLeitura: lido ? valores.dataFimLeitura || null : null,
      avaliacao: lido ? valores.avaliacao : null
    };

    return this.api.cadastrar(livro);
  }

  private atualizar(id: string): Observable<Livro> {
    const valores = this.form.getRawValue();

    const dados: AtualizarLivro = {
      titulo: valores.titulo,
      autor: valores.autor,
      formato: valores.formato,
      isbn: valores.isbn || null,
      genero: valores.genero || null,
      totalPaginas: valores.totalPaginas,
      capaUrl: valores.capaUrl.trim() || null
    };

    return this.api.atualizarDados(id, dados).pipe(
      switchMap(() => this.api.atualizarAnotacoes(id, valores.anotacoes || null))
    );
  }
}
