import {
  HttpClient,
  HttpEvent,
  HttpParams,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/Operators';
import { environment } from 'src/environments/environment';
import { ValorSeleccion } from '../_dto/valorSeleccion';
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
    procesado?: number,
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
    if (procesado != null) {
      params = params.append('procesado', procesado.toString());
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

  ObtenerListaDetalleLiquidacionTotal(
    listaEstadoId: string,
    terceroId?: number,
    procesado?: number
  ): Observable<number[]> {
    const path = 'ObtenerListaDetalleLiquidacionTotal';

    let params = new HttpParams();
    params = params.append('listaEstadoId', listaEstadoId);
    if (terceroId > 0) {
      params = params.append('terceroId', terceroId.toString());
    }
    if (procesado != null) {
      params = params.append('procesado', procesado.toString());
    }

    return this.http.get<number[]>(this.baseUrl + path, {
      params,
    });
  }

  RegistrarDetalleLiquidacion(
    formato: FormatoCausacionyLiquidacionPago
  ): Observable<any> {
    const path = 'RegistrarDetalleLiquidacion';
    return this.http.post(this.baseUrl + path, formato);
  }

  RegistrarListaDetalleLiquidacion(
    listaPlanPagoId: string,
    listaEstadoId: string,
    seleccionarTodo: number,
    terceroId?: number 
  ): Observable<any> {
    const path = 'RegistrarListaDetalleLiquidacion';

    let params = new HttpParams();
    if (seleccionarTodo > 0) {
      params = params.append('seleccionarTodo', seleccionarTodo.toString());
    }
    if (terceroId > 0) {
      params = params.append('terceroId', terceroId.toString());
    }
    params = params.append('listaPlanPagoId', listaPlanPagoId);
    params = params.append('listaEstadoId', listaEstadoId);

    return this.http.get(this.baseUrl + path, { params });
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
    valorBaseGravable: number,
    actividadEconomicaId: number
  ): Observable<FormatoCausacionyLiquidacionPago> {
    const path = 'ObtenerFormatoCausacionyLiquidacionPago';

    let params = new HttpParams();

    if (planPagoId > 0) {
      params = params.append('planPagoId', planPagoId.toString());
    }
    if (valorBaseGravable > 0) {
      params = params.append('valorBaseGravable', valorBaseGravable.toString());
    }

    if (actividadEconomicaId > 0) {
      params = params.append(
        'actividadEconomicaId',
        actividadEconomicaId.toString()
      );
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

  public DescargarMaestroDetalleLiquidacionParaArchivo(
    listaLiquidacionId: string
  ): Observable<HttpEvent<Blob>> {
    return this.http.request(
      new HttpRequest(
        'GET',
        `${
          this.baseUrl + 'DescargarMaestroDetalleLiquidacionParaArchivo'
        }?listaLiquidacionId=${listaLiquidacionId}`,
        null,
        {
          reportProgress: true,
          responseType: 'blob',
        }
      )
    );
  }

  public DescargarDetalleLiquidacionParaArchivo(
    listaLiquidacionId: string
  ): Observable<HttpEvent<Blob>> {
    return this.http.request(
      new HttpRequest(
        'GET',
        `${
          this.baseUrl + 'DescargarDetalleLiquidacionParaArchivo'
        }?listaLiquidacionId=${listaLiquidacionId}`,
        null,
        {
          reportProgress: true,
          responseType: 'blob',
        }
      )
    );
  }

  ObtenerListaActividadesEconomicaXTercero(
    terceroId: number
  ): Observable<ValorSeleccion[]> {
    let params = new HttpParams();

    if (terceroId > 0) {
      params = params.append('terceroId', terceroId.toString());
    }

    const path = 'ObtenerListaActividadesEconomicaXTercero';
    return this.http.get<ValorSeleccion[]>(this.baseUrl + path, { params });
  }
}
