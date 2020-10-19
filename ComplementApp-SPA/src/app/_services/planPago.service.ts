import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable, of as observableOf } from 'rxjs';
import { PlanPago } from '../_models/planPago';
import { DetallePlanPago } from '../_models/detallePlanPago';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { FormatoCausacionyLiquidacionPago } from '../_models/formatoCausacionyLiquidacionPago';

@Injectable({
  providedIn: 'root',
})
export class PlanPagoService {
  baseUrl = environment.apiUrl + 'planpago/';

  constructor(private http: HttpClient) {}

  ObtenerListaPlanPago(
    listaEstadoId: string,
    terceroId?: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<PlanPago[]>> {
    const path = 'ObtenerListaPlanPago';
    const paginatedResult: PaginatedResult<PlanPago[]> = new PaginatedResult<
      PlanPago[]
    >();

    let params = new HttpParams();
    params = params.append('listaEstadoId', listaEstadoId);
    if (terceroId > 0) {
      params = params.append('terceroId', terceroId.toString());
    }
    if (page != null) {
      params = params.append('pageNumber', page);
    }
    if (pagesize != null) {
      params = params.append('pageSize', pagesize);
    }

    return this.http
      .get<PlanPago[]>(this.baseUrl + path, {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          paginatedResult.result = response.body;

          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }
          return paginatedResult;
        })
      );
  }

  ObtenerListaDetalleLiquidacion(
    listaEstadoId: string,
    terceroId?: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<FormatoCausacionyLiquidacionPago[]>> {
    const path = 'ObtenerListaDetalleLiquidacion';
    const paginatedResult: PaginatedResult<FormatoCausacionyLiquidacionPago[]> = new PaginatedResult<
    FormatoCausacionyLiquidacionPago[]
    >();

    let params = new HttpParams();
    params = params.append('listaEstadoId', listaEstadoId);
    if (terceroId > 0) {
      params = params.append('terceroId', terceroId.toString());
    }
    if (page != null) {
      params = params.append('pageNumber', page);
    }
    if (pagesize != null) {
      params = params.append('pageSize', pagesize);
    }

    return this.http
      .get<FormatoCausacionyLiquidacionPago[]>(this.baseUrl + path, {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          paginatedResult.result = response.body;

          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }
          return paginatedResult;
        })
      );
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

  ObtenerFormatoCausacionyLiquidacionPago(
    planPagoId: number,
    valorBaseGravable: number
  ): Observable<FormatoCausacionyLiquidacionPago> {
    const path = 'ObtenerFormatoCausacionyLiquidacionPago';

    let params = new HttpParams();

    if (planPagoId > 0) {
      params = params.append('planPagoId', planPagoId.toString());
    }
    if (valorBaseGravable > 0) {
      params = params.append('valorBaseGravable', valorBaseGravable.toString());
    }

    return this.http.get<FormatoCausacionyLiquidacionPago>(
      this.baseUrl + path,
      { params }
    );
  }

  ObtenerDetalleFormatoCausacionyLiquidacionPago(
    detalleLiquidacionId: number
  ): Observable<FormatoCausacionyLiquidacionPago> {
    const path = 'ObtenerDetalleFormatoCausacionyLiquidacionPago/';
    return this.http.get<FormatoCausacionyLiquidacionPago>(
      this.baseUrl + path + detalleLiquidacionId
    );
  }

  ActualizarPlanPago(factura: PlanPago): Observable<boolean> {
    this.http.put(this.baseUrl, factura).subscribe(() => {});
    return observableOf(true);
  }

  RegistrarDetalleLiquidacion(
    formato: FormatoCausacionyLiquidacionPago
  ): Observable<any> {
    const path = 'RegistrarDetalleLiquidacion';
    return this.http.post(this.baseUrl + path, formato);
  }

  RechazarDetalleLiquidacion(
    planPagoId: number,
    mensajeRechazo: string
  ): Observable<any> {
    const path = 'RechazarDetalleLiquidacion/';
    return this.http.get(this.baseUrl + path + planPagoId + '/' + mensajeRechazo);
  }
}
