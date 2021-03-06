import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import {
  HttpClient,
  HttpEvent,
  HttpParams,
  HttpRequest,
} from '@angular/common/http';
import { Observable, of as observableOf } from 'rxjs';
import { PlanPago } from '../_models/planPago';
import { DetallePlanPago } from '../_models/detallePlanPago';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { FormatoCausacionyLiquidacionPago } from '../_models/formatoCausacionyLiquidacionPago';
import { RadicadoDto } from '../_dto/radicadoDto';
import { FormaPagoCompromiso } from '../_models/formaPagoCompromiso';
import { Cdp } from '../_models/cdp';
import { LineaPlanPagoDto } from '../_dto/LineaPlanPagoDto';

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

  ObtenerListaPlanPagoXCompromiso(
    crp: number,
    listaEstadoId: string,
    page?,
    pagesize?
  ): Observable<PaginatedResult<PlanPago[]>> {
    const path = 'ObtenerListaPlanPagoXCompromiso';
    const paginatedResult: PaginatedResult<PlanPago[]> = new PaginatedResult<
      PlanPago[]
    >();

    let params = new HttpParams();
    params = params.append('listaEstadoId', listaEstadoId);
    params = params.append('crp', crp.toString());

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

  ObtenerDetallePlanPagoParaSolicitudPago(planPagoId: number): Observable<DetallePlanPago> {
    const path = 'ObtenerDetallePlanPagoParaSolicitudPago';

    let params = new HttpParams();

    if (planPagoId > 0) {
      params = params.append('planPagoId', planPagoId.toString());
    }

    return this.http.get<DetallePlanPago>(this.baseUrl + path, { params });
  }


  ActualizarPlanPago(factura: PlanPago): Observable<boolean> {
    this.http.put(this.baseUrl, factura).subscribe(() => {});
    return observableOf(true);
  }

  ObtenerListaRadicadoPaginado(
    mes: number,
    listaEstadoId: string,
    terceroId?: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<RadicadoDto[]>> {
    const path = 'ObtenerListaRadicadoPaginado';
    const paginatedResult: PaginatedResult<RadicadoDto[]> = new PaginatedResult<
      RadicadoDto[]
    >();

    let params = new HttpParams();
    params = params.append('mes', mes.toString());
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
      .get<RadicadoDto[]>(this.baseUrl + path, {
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

  DescargarListaRadicado(
    mes: number,
    listaEstadoId: string,
    terceroId?: number
  ): Observable<HttpEvent<Blob>> {
    return this.http.request(
      new HttpRequest(
        'GET',
        `${
          this.baseUrl + 'DescargarListaRadicado'
        }?mes=${mes}&listaEstadoId=${listaEstadoId}&terceroId=${terceroId}`,
        null,
        {
          reportProgress: true,
          responseType: 'blob',
        }
      )
    );
  }

  DescargarListaPlanPago(): Observable<HttpEvent<Blob>> {
    return this.http.request(
      new HttpRequest(
        'GET',
        `${
          this.baseUrl + 'DescargarListaPlanPago'
        }`,
        null,
        {
          reportProgress: true,
          responseType: 'blob',
        }
      )
    );
  }

  RegistrarFormaPagoCompromiso(
    tipo: number,
    formaPagoCompromiso: FormaPagoCompromiso
  ): Observable<any> {
    const path = 'RegistrarFormaPagoCompromiso';
    let params = new HttpParams();
    params = params.append('tipo', tipo.toString());
    return this.http.post(this.baseUrl + path, formaPagoCompromiso, { params });
  }

  ObtenerCompromisosParaPlanPago(
    tipo: number,
    terceroId: number,
    numeroCrp: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<Cdp[]>> {
    const paginatedResult: PaginatedResult<Cdp[]> = new PaginatedResult<
      Cdp[]
    >();

    const path = 'ObtenerCompromisosParaPlanPago';

    let params = new HttpParams();
    params = params.append('tipo', tipo.toString());
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

  ObtenerLineasPlanPagoXCompromiso(
    crp: number
  ): Observable<LineaPlanPagoDto[]> {
    const paginatedResult: PaginatedResult<Cdp[]> = new PaginatedResult<
      Cdp[]
    >();

    const path = 'ObtenerLineasPlanPagoXCompromiso';

    let params = new HttpParams();
    params = params.append('crp', crp.toString());

    return this.http.get<LineaPlanPagoDto[]>(this.baseUrl + path, { params });
  }

  ActualizarFormaPagoCompromiso(
    formaPagoCompromiso: FormaPagoCompromiso
  ): Observable<boolean> {
    const path = 'ActualizarFormaPagoCompromiso';
    this.http.put(this.baseUrl + path, formaPagoCompromiso).subscribe(() => {});
    return observableOf(true);
  }
}
