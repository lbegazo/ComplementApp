import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/Operators';
import { environment } from 'src/environments/environment';
import { ClavePresupuestalContable } from '../_dto/clavePresupuestalContable';
import { FormatoSolicitudPagoDto } from '../_dto/formatoSolicitudPagoDto';
import { Cdp } from '../_models/cdp';
import { FormatoCausacionyLiquidacionPago } from '../_models/formatoCausacionyLiquidacionPago';
import { FormatoSolicitudPago } from '../_models/formatoSolicitudPago';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root',
})
export class ObligacionService {
  baseUrl = environment.apiUrl + 'Obligacion/';

  constructor(private http: HttpClient) {}

  ObtenerCompromisosParaSolicitudRegistroPago(
    usuarioId: number,
    perfilId: number,
    terceroId: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<Cdp[]>> {
    const paginatedResult: PaginatedResult<Cdp[]> = new PaginatedResult<
      Cdp[]
    >();

    const path = 'ObtenerCompromisosParaSolicitudRegistroPago';

    let params = new HttpParams();
    if (usuarioId > 0) {
      params = params.append('usuarioId', usuarioId.toString());
    }
    if (perfilId > 0) {
      params = params.append('perfilId', perfilId.toString());
    }
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
      .get<Cdp[]>(this.baseUrl + path, { observe: 'response', params })
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

  ObtenerSolicitudesPagoParaAprobar(
    usuarioId: number,
    terceroId: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<Cdp[]>> {
    const paginatedResult: PaginatedResult<Cdp[]> = new PaginatedResult<
      Cdp[]
    >();

    const path = 'ObtenerSolicitudesPagoParaAprobar';

    let params = new HttpParams();
    if (usuarioId > 0) {
      params = params.append('usuarioId', usuarioId.toString());
    }
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
      .get<Cdp[]>(this.baseUrl + path, { observe: 'response', params })
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

  ActualizarFormatoSolicitudPago(
    formato: FormatoSolicitudPagoDto
  ): Observable<any> {
    const path = 'ActualizarFormatoSolicitudPago';
    return this.http.put(this.baseUrl + path, formato);
  }

  ObtenerCompromisosParaClavePresupuestalContable(
    terceroId: number,
    numeroCrp: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<Cdp[]>> {
    const paginatedResult: PaginatedResult<Cdp[]> = new PaginatedResult<
      Cdp[]
    >();

    const path = 'ObtenerCompromisosParaClavePresupuestalContable';

    let params = new HttpParams();
    if (terceroId > 0) {
      params = params.append('terceroId', terceroId.toString());
    }
    if (numeroCrp > 0) {
      params = params.append('numeroCrp', numeroCrp.toString());
    }
    if (page != null) {
      params = params.append('pageNumber', page);
    }
    if (pagesize != null) {
      params = params.append('pageSize', pagesize);
    }

    return this.http
      .get<Cdp[]>(this.baseUrl + path, { observe: 'response', params })
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

  ObtenerRubrosParaClavePresupuestalContable(
    cdpId: number
  ): Observable<ClavePresupuestalContable[]> {
    const path = 'ObtenerRubrosParaClavePresupuestalContable';

    let params = new HttpParams();

    params = params.append('cdpId', cdpId.toString());

    return this.http.get<ClavePresupuestalContable[]>(this.baseUrl + path, {
      params,
    });
  }

  ObtenerRelacionesContableXRubro(
    rubroPresupuestalId: number
  ): Observable<Cdp[]> {
    const path = 'ObtenerRelacionesContableXRubro';

    let params = new HttpParams();

    params = params.append(
      'rubroPresupuestalId',
      rubroPresupuestalId.toString()
    );

    return this.http.get<Cdp[]>(this.baseUrl + path, { params });
  }

  ObtenerFormatoSolicitudPago(
    cdpId: number
  ): Observable<FormatoSolicitudPagoDto> {
    const path = 'ObtenerFormatoSolicitudPago';

    let params = new HttpParams();
    params = params.append('cdpId', cdpId.toString());

    return this.http.get<FormatoSolicitudPagoDto>(this.baseUrl + path, {
      params,
    });
  }

  ObtenerFormatoSolicitudPagoXId(
    formatoSolicitudPagoId: number
  ): Observable<FormatoSolicitudPagoDto> {
    const path = 'ObtenerFormatoSolicitudPagoXId';

    let params = new HttpParams();
    params = params.append(
      'formatoSolicitudPagoId',
      formatoSolicitudPagoId.toString()
    );

    return this.http.get<FormatoSolicitudPagoDto>(this.baseUrl + path, {
      params,
    });
  }

  RegistrarFormatoSolicitudPago(
    formato: FormatoSolicitudPago
  ): Observable<any> {
    const path = 'RegistrarFormatoSolicitudPago';
    return this.http.post(this.baseUrl + path, formato);
  }

  ObtenerSeguridadSocialParaSolicitudPago(
    planPagoId: number,
    valorBaseCotizacion: number,
    actividadEconomicaId: number
  ): Observable<FormatoCausacionyLiquidacionPago> {
    const path = 'ObtenerSeguridadSocialParaSolicitudPago';

    let params = new HttpParams();

    if (planPagoId > 0) {
      params = params.append('planPagoId', planPagoId.toString());
    }
    params = params.append(
      'valorBaseCotizacion',
      valorBaseCotizacion.toString()
    );

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
}
