import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/Operators';
import { environment } from 'src/environments/environment';
import { FormatoCausacionyLiquidacionPago } from '../_models/formatoCausacionyLiquidacionPago';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root',
})
export class DetalleLiquidacionService {
  baseUrl = environment.apiUrl + 'detalleliquidacion/';

  constructor(private http: HttpClient) {}

  ObtenerListaDetalleLiquidacion(
    listaEstadoId: string,
    terceroId?: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<FormatoCausacionyLiquidacionPago[]>> {
    const path = 'ObtenerListaDetalleLiquidacion';
    const paginatedResult: PaginatedResult<
      FormatoCausacionyLiquidacionPago[]
    > = new PaginatedResult<FormatoCausacionyLiquidacionPago[]>();

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
    return this.http.get(
      this.baseUrl + path + planPagoId + '/' + mensajeRechazo
    );
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
}
