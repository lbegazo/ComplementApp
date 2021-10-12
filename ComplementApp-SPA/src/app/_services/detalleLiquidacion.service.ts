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
import { EnvioParametroDto } from '../_dto/envioParametroDto';
import { RespuestaSolicitudPago } from '../_dto/respuestaSolicitudPago';
import { ValorSeleccion } from '../_dto/valorSeleccion';
import { FormatoCausacionyLiquidacionPago } from '../_models/formatoCausacionyLiquidacionPago';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root',
})
export class DetalleLiquidacionService {
  baseUrl = environment.apiUrl + 'detalleliquidacion/';

  constructor(private http: HttpClient) {}

  ObtenerLiquidacionesParaCuentaPorPagarArchivo(
    listaEstadoId: string,
    terceroId?: number,
    procesado?: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<FormatoCausacionyLiquidacionPago[]>> {
    const path = 'ObtenerLiquidacionesParaCuentaPorPagarArchivo';
    const paginatedResult: PaginatedResult<FormatoCausacionyLiquidacionPago[]> =
      new PaginatedResult<FormatoCausacionyLiquidacionPago[]>();

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
    listaSolicitudPagoId: string,
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
    params = params.append('listaSolicitudPagoId', listaSolicitudPagoId);
    params = params.append('listaEstadoId', listaEstadoId);

    return this.http.get(this.baseUrl + path, { params });
  }

  RechazarDetalleLiquidacion(
    planPagoId: number,
    solicitudPagoId: number,
    mensajeRechazo: string
  ): Observable<any> {
    const path = 'RechazarDetalleLiquidacion/';
    let params = new HttpParams();
    params = params.append('solicitudPagoId', solicitudPagoId.toString());
    params = params.append('planPagoId', planPagoId.toString());
    params = params.append('mensajeRechazo', mensajeRechazo);

    return this.http.get(this.baseUrl + path, { params });
  }

  RechazarLiquidacion(
    detalleLiquidacionId: number,
    planPagoId: number,
    solicitudPagoId: number,
    mensajeRechazo: string
  ): Observable<any> {
    const path = 'RechazarLiquidacion/';
    let params = new HttpParams();
    params = params.append(
      'detalleLiquidacionId',
      detalleLiquidacionId.toString()
    );
    params = params.append('solicitudPagoId', solicitudPagoId.toString());
    params = params.append('planPagoId', planPagoId.toString());
    params = params.append('mensajeRechazo', mensajeRechazo);

    return this.http.get(this.baseUrl + path, { params });
  }

  ObtenerFormatoCausacionyLiquidacionPago(
    solicitudPagoId: number,
    planPagoId: number,
    valorBaseGravable: number,
    actividadEconomicaId: number
  ): Observable<FormatoCausacionyLiquidacionPago> {
    const path = 'ObtenerFormatoCausacionyLiquidacionPago';

    let params = new HttpParams();
    if (solicitudPagoId > 0) {
      params = params.append('solicitudPagoId', solicitudPagoId.toString());
    }
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

  //#region Creación Archivo Cuenta Por Pagar

  DescargarCabeceraArchivoLiquidacionCuentaPorPagar(
    listaLiquidacionId: string
  ): Observable<HttpEvent<Blob>> {
    return this.http.request(
      new HttpRequest(
        'GET',
        `${
          this.baseUrl + 'DescargarCabeceraArchivoLiquidacionCuentaPorPagar'
        }?listaLiquidacionId=${listaLiquidacionId}`,
        null,
        {
          reportProgress: true,
          responseType: 'blob',
        }
      )
    );
  }

  DescargarDetalleArchivoLiquidacionCuentaPorPagar(
    listaLiquidacionId: string
  ): Observable<HttpEvent<Blob>> {
    return this.http.request(
      new HttpRequest(
        'GET',
        `${
          this.baseUrl + 'DescargarDetalleArchivoLiquidacionCuentaPorPagar'
        }?listaLiquidacionId=${listaLiquidacionId}`,
        null,
        {
          reportProgress: true,
          responseType: 'blob',
        }
      )
    );
  }

  //#endregion Creación Archivo Cuenta Por Pagar

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

  //#region Creacion Archivo Obligacion

  ObtenerLiquidacionesParaArchivoObligacion(
    listaEstadoId: string,
    terceroId?: number,
    procesado?: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<FormatoCausacionyLiquidacionPago[]>> {
    const path = 'ObtenerLiquidacionesParaArchivoObligacion';
    const paginatedResult: PaginatedResult<FormatoCausacionyLiquidacionPago[]> =
      new PaginatedResult<FormatoCausacionyLiquidacionPago[]>();

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

  ObtenerLiquidacionIdsParaObligacionArchivo(
    listaEstadoId: string,
    terceroId?: number,
    procesado?: number,
    page?,
    pagesize?
  ): Observable<number[]> {
    const path = 'ObtenerLiquidacionIdsParaObligacionArchivo';

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

    return this.http.get<number[]>(this.baseUrl + path, {
      params,
    });
  }

  public DescargarListaDetalleLiquidacion(
    listaEstadoId: string,
    terceroId?: number,
    procesado?: number,
    page?,
    pagesize?
  ): Observable<HttpEvent<Blob>> {
    let parametros = new HttpParams();
    parametros = parametros.append('listaEstadoId', listaEstadoId);
    if (terceroId > 0) {
      parametros = parametros.append('terceroId', terceroId.toString());
    }
    if (procesado != null) {
      parametros = parametros.append('procesado', procesado.toString());
    }
    if (page != null) {
      parametros = parametros.append('pageNumber', page);
    }
    if (pagesize != null) {
      parametros = parametros.append('pageSize', pagesize);
    }
    return this.http.request(
      new HttpRequest(
        'GET',
        `${this.baseUrl + 'DescargarListaDetalleLiquidacion'}`,
        null,
        {
          reportProgress: true,
          responseType: 'blob',
          params: parametros,
        }
      )
    );
  }

  ValidarLiquidacionSinClavePresupuestal(
    listaEstadoId: string,
    terceroId?: number
  ): Observable<RespuestaSolicitudPago> {
    const path = 'ValidarLiquidacionSinClavePresupuestal';

    let params = new HttpParams();
    params = params.append('listaEstadoId', listaEstadoId);
    if (terceroId > 0) {
      params = params.append('terceroId', terceroId.toString());
    }

    return this.http.get<RespuestaSolicitudPago>(this.baseUrl + path, {
      params,
    });
  }

  ObtenerListaLiquidacionIdParaArchivo(
    listaLiquidacionId: string,
    listaEstadoId: string,
    terceroId: number,
    conRubroFuncionamiento: number,
    conRubroUsoPresupuestal: number
  ): Observable<RespuestaSolicitudPago> {
    const path = 'ObtenerListaLiquidacionIdParaArchivo';

    let params = new HttpParams();
    params = params.append('listaEstadoId', listaEstadoId);

    params = params.append(
      'conRubroFuncionamiento',
      conRubroFuncionamiento.toString()
    );
    params = params.append(
      'conRubroUsoPresupuestal',
      conRubroUsoPresupuestal.toString()
    );

    if (listaLiquidacionId.length > 0) {
      params = params.append(
        'listaLiquidacionId',
        listaLiquidacionId.toString()
      );
    }
    if (terceroId > 0) {
      params = params.append('terceroId', terceroId.toString());
    }

    return this.http.get<RespuestaSolicitudPago>(this.baseUrl + path, {
      params,
    });
  }

  public DescargarArchivoLiquidacionObligacion(
    listaLiquidacionId: string,
    tipoArchivoObligacionId: number
  ): Observable<HttpEvent<Blob>> {
    let params = new HttpParams();

    params = params.append(
      'tipoArchivoObligacionId',
      tipoArchivoObligacionId.toString()
    );

    if (listaLiquidacionId.length > 0) {
      params = params.append(
        'listaLiquidacionId',
        listaLiquidacionId.toString()
      );
    }

    return this.http.request(
      new HttpRequest(
        'GET',
        `${this.baseUrl + 'DescargarArchivoLiquidacionObligacion'}`,
        null,
        {
          reportProgress: true,
          responseType: 'blob',
          params,
        }
      )
    );
  }

  //#endregion Creacion Archivo Obligacion

  //#region Archivo General

  ObtenerListaArchivoCreados(
    param: EnvioParametroDto
  ): Observable<ValorSeleccion[]> {
    const path = 'ObtenerListaArchivoCreados';

    return this.http.put<ValorSeleccion[]>(this.baseUrl + path, param);
  }

  ObtenerDocumentosParaAdministracionArchivo(
    archivoId: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<FormatoCausacionyLiquidacionPago[]>> {
    const path = 'ObtenerDocumentosParaAdministracionArchivo';
    const paginatedResult: PaginatedResult<FormatoCausacionyLiquidacionPago[]> =
      new PaginatedResult<FormatoCausacionyLiquidacionPago[]>();

    let params = new HttpParams();
    params = params.append('archivoId', archivoId.toString());

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

  ActualizarListaLiquidacionDeArchivo(
    archivoId: number,
    listaLiquidacionId: string,
    seleccionarTodo: number
  ): Observable<any> {
    const path = 'ActualizarListaLiquidacionDeArchivo';

    let params = new HttpParams();
    params = params.append('archivoId', archivoId.toString());
    if (seleccionarTodo > 0) {
      params = params.append('seleccionarTodo', seleccionarTodo.toString());
    }
    if (listaLiquidacionId.length > 0) {
      params = params.append(
        'listaLiquidacionId',
        listaLiquidacionId.toString()
      );
    }

    return this.http.get(this.baseUrl + path, { params });
  }

  //#endregion  Archivo General
}
