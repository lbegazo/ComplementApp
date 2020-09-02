import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable, of as observableOf } from 'rxjs';
import { PlanPago } from '../_models/planPago';
import { DetallePlanPago } from '../_models/detallePlanPago';

@Injectable({
  providedIn: 'root',
})
export class FacturaService {
  baseUrl = environment.apiUrl + 'planpago/';

  constructor(private http: HttpClient) {}

  //Se comenta el codigo, se precisa investigar mas para saber como enviar un dto a una web api
  // ObtenerListaPlanPago(filtroFactura: FiltroFactura): Observable<PlanPago[]> {
  //   const path = 'planpago/ObtenerListaPlanPago';

  //   let params = new HttpParams();

  //   if (filtroFactura) {
  //     params = params.append('filtro', JSON.stringify(filtroFactura));
  //   }

  //   return this.http.get<PlanPago[]>(this.baseUrl + path, { params });
  // }

  ObtenerListaPlanPago(
    terceroId: number,
    listaEstadoId: string
  ): Observable<PlanPago[]> {
    const path = 'ObtenerListaPlanPago';

    let params = new HttpParams();

    if (terceroId > 0 && listaEstadoId.length > 0) {
      params = params.append('terceroId', terceroId.toString());
      params = params.append('listaEstadoId', listaEstadoId);
    }

    return this.http.get<PlanPago[]>(this.baseUrl + path, { params });
  }

  ObtenerPlanPago(planPagoId: number): Observable<PlanPago> {
    const path = 'ObtenerPlanPago';

    let params = new HttpParams();

    if (planPagoId > 0) {
      params = params.append('planPagoId', planPagoId.toString());
    }

    return this.http.get<PlanPago>(this.baseUrl + path, { params });
  }

  ObtenerDetallePlanPago(planPagoId: number): Observable<DetallePlanPago> {
    const path = 'ObtenerDetallePlanPago';

    let params = new HttpParams();

    if (planPagoId > 0) {
      params = params.append('planPagoId', planPagoId.toString());
    }

    return this.http.get<DetallePlanPago>(this.baseUrl + path, { params });
  }

  ActualizarPlanPago(
    planPagoId: number,
    factura: PlanPago
  ): Observable<boolean> {
    this.http.put(this.baseUrl, factura).subscribe(() => {});
    return observableOf(true);
  }
}
