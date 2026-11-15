import { Component, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { SwUpdate } from '@angular/service-worker';
import { filter, map, merge } from 'rxjs';

@Component({
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
  host: { '(document:visibilitychange)': 'procurarVersaoNova()' },
})
export class App {
  private readonly atualizacoes = inject(SwUpdate);

  protected readonly titulo = signal('Minha Estante');

  // Depois de uma nova publicação, o service worker baixa a versão nova em segundo plano
  // e avisa aqui. Recarregar a página passa a usar a versão nova.
  protected readonly avisoDeVersao = toSignal(
    merge(
      this.atualizacoes.versionUpdates.pipe(
        filter((evento) => evento.type === 'VERSION_READY'),
        map(() => 'Há uma versão nova da Minha Estante.'),
      ),
      this.atualizacoes.unrecoverable.pipe(
        map(() => 'A Minha Estante precisa ser recarregada para continuar funcionando.'),
      ),
    ),
    { initialValue: null },
  );

  // Quando você volta para a janela do app, ele confere se há uma versão nova.
  protected procurarVersaoNova(): void {
    if (this.atualizacoes.isEnabled && document.visibilityState === 'visible') {
      this.atualizacoes.checkForUpdate().catch(() => {
        // O servidor não respondeu (por exemplo, no meio de uma publicação). Confere na próxima vez.
      });
    }
  }

  protected recarregar(): void {
    document.location.reload();
  }
}
