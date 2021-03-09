import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import {
  HttpClient,
  HttpEvent,
  HttpParams,
  HttpRequest,
} from '@angular/common/http';
import { Observable, of as observableOf } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Cdp } from '../_models/cdp';
import { Contrato } from '../_models/contrato';

@Injectable({
  providedIn: 'root',
})
export class ContratoService {
  baseUrl = environment.apiUrl + 'contrato/';

  constructor(private http: HttpClient) {}

  ObtenerCompromisosParaContrato(
    tipo: number,
    terceroId: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<Cdp[]>> {
    const paginatedResult: PaginatedResult<Cdp[]> = new PaginatedResult<
      Cdp[]
    >();

    const path = 'ObtenerCompromisosParaContrato';

    let params = new HttpParams();
    params = params.append('tipo', tipo.toString());
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

  ObtenerContrato(contratoId: number): Observable<Contrato> {
    const path = 'ObtenerContrato';
    let params = new HttpParams();
    params = params.append('contratoId', contratoId.toString());
    return this.http.get<Contrato>(this.baseUrl + path, {
      params,
    });
  }

  ActualizarContrato(contrato: Contrato) {
    const path = 'ActualizarContrato';
    return this.http.put(this.baseUrl + path, contrato);
  }

  RegistrarContrato(contrato: Contrato): Observable<any> {
    const path = 'RegistrarContrato';
    return this.http.post(this.baseUrl + path, contrato);
  }

  public DescargarListaContratoTotal(): Observable<HttpEvent<Blob>> {
    return this.http.request(
      new HttpRequest(
        'GET',
        `${
          this.baseUrl + 'DescargarListaContratoTotal'
        }`,
        null,
        {
          reportProgress: true,
          responseType: 'blob',
        }
      )
    );
  }
}
