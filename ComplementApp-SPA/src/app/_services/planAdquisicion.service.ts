import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { DetalleCDP } from '../_models/detalleCDP';

@Injectable({
  providedIn: 'root'
})
export class PlanAdquisicionService {
  baseUrl = environment.apiUrl + 'planAdquisicion/';

  constructor(private http: HttpClient) {}

  ObtenerListaPlanAnualAdquisicion(): Observable<DetalleCDP[]> {
    const path = 'ObtenerListaPlanAnualAdquisicion';
    return this.http.get<DetalleCDP[]>(this.baseUrl + path, {});
  }

  RegistrarPlanAdquisicion(
    planAdquisicion: DetalleCDP
  ): Observable<any> {
    const path = 'RegistrarPlanAdquisicion';
    return this.http.post(this.baseUrl + path, planAdquisicion);
  }
}
