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
import { DetalleCDP } from '../_models/detalleCDP';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root',
})
export class PlanAdquisicionService {
  baseUrl = environment.apiUrl + 'planAdquisicion/';

  constructor(private http: HttpClient) {}

  ObtenerListaPlanAnualAdquisicion(): Observable<DetalleCDP[]> {
    const path = 'ObtenerListaPlanAnualAdquisicion';
    return this.http.get<DetalleCDP[]>(this.baseUrl + path, {});
  }

  RegistrarPlanAdquisicion(planAdquisicion: DetalleCDP): Observable<any> {
    const path = 'RegistrarPlanAdquisicion';
    return this.http.post(this.baseUrl + path, planAdquisicion);
  }

  ObtenerListaPlanAdquisicionSinCDPXIds(
    listaPlanAdquisicionId: string,
    seleccionarTodo: number,
    rubroPresupuestalId: number
  ): Observable<DetalleCDP[]> {
    const path = 'ObtenerListaPlanAdquisicionSinCDPXIds';

    let params = new HttpParams();
    if (listaPlanAdquisicionId.length > 0) {
      params = params.append(
        'listaPlanAdquisicionId',
        listaPlanAdquisicionId.toString()
      );
    }

    if (seleccionarTodo > 0) {
      params = params.append('seleccionarTodo', seleccionarTodo.toString());
    }

    if (rubroPresupuestalId > 0) {
      params = params.append(
        'rubroPresupuestalId',
        rubroPresupuestalId.toString()
      );
    }

    return this.http.get<DetalleCDP[]>(this.baseUrl + path, { params });
  }

  ObtenerListaPlanAnualAdquisicionPaginada(
    esCreacion: number,
    rubroPresupuestalId: number,
    numeroCdp: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<DetalleCDP[]>> {
    const path = 'ObtenerListaPlanAnualAdquisicionPaginada';
    const paginatedResult: PaginatedResult<DetalleCDP[]> = new PaginatedResult<
      DetalleCDP[]
    >();

    let params = new HttpParams();
    params = params.append('esCreacion', esCreacion.toString());

    if (rubroPresupuestalId > 0) {
      params = params.append(
        'rubroPresupuestalId',
        rubroPresupuestalId.toString()
      );
    }

    if (numeroCdp > 0) {
      params = params.append('numeroCdp', numeroCdp.toString());
    }

    if (page != null) {
      params = params.append('pageNumber', page);
    }
    if (pagesize != null) {
      params = params.append('pageSize', pagesize);
    }

    return this.http
      .get<DetalleCDP[]>(this.baseUrl + path, {
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

  ObtenerListaPlanAdquisicionReporte(
    rubroPresupuestalId?: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<DetalleCDP[]>> {
    const path = 'ObtenerListaPlanAdquisicionReporte';
    const paginatedResult: PaginatedResult<DetalleCDP[]> = new PaginatedResult<
      DetalleCDP[]
    >();

    let params = new HttpParams();
    if (rubroPresupuestalId > 0) {
      params = params.append(
        'rubroPresupuestalId',
        rubroPresupuestalId.toString()
      );
    }

    if (page != null) {
      params = params.append('pageNumber', page);
    }
    if (pagesize != null) {
      params = params.append('pageSize', pagesize);
    }

    return this.http
      .get<DetalleCDP[]>(this.baseUrl + path, {
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

  public DescargarListaPlanAnualAdquisicion(
    rubroPresupuestalId?: number
  ): Observable<HttpEvent<Blob>> {
    let params = new HttpParams();
    if (rubroPresupuestalId > 0) {
      params = params.append(
        'rubroPresupuestalId',
        rubroPresupuestalId.toString()
      );
    }

    return this.http.request(
      new HttpRequest(
        'GET',
        `${this.baseUrl + 'DescargarListaPlanAnualAdquisicion'}`,
        null,
        {
          params,
          reportProgress: true,
          responseType: 'blob',
        }
      )
    );
  }
}
