import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Estatisticas } from './estatisticas';

@Injectable({ providedIn: 'root' })
export class EstatisticasApi {
  private readonly http = inject(HttpClient);

  obter(): Observable<Estatisticas> {
    return this.http.get<Estatisticas>(`${environment.apiUrl}/estatisticas`);
  }
}
