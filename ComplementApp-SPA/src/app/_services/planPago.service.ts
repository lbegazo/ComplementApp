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

  public DescargarListaRadicado(
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
}
