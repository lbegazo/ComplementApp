import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { Cdp } from '../_models/cdp';
import { HttpClient, HttpEvent, HttpParams, HttpRequest } from '@angular/common/http';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/Operators';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CargaObligacionService {
  baseUrl = environment.apiUrl + 'CargaObligacion/';

  constructor(private http: HttpClient) {}

  ObtenerListaCargaObligacion(
    estado: string,
    page?,
    pagesize?
  ): Observable<PaginatedResult<Cdp[]>> {
    let params = new HttpParams();
    const path = 'ObtenerListaCargaObligacion';
    const paginatedResult: PaginatedResult<Cdp[]> = new PaginatedResult<
      Cdp[]
    >();

    params = params.append('estado', estado);
    if (page != null) {
      params = params.append('pageNumber', page);
    }
    if (pagesize != null) {
      params = params.append('pageSize', pagesize);
    }

    return this.http
      .get<Cdp[]>(this.baseUrl + path, {
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

  public DescargarArchivoCargaObligacion(
    tipoArchivoId: number,
    estado: string
  ): Observable<HttpEvent<Blob>> {
    let params = new HttpParams();

    params = params.append('tipoArchivoId', tipoArchivoId.toString());
    params = params.append('estado', estado);

    return this.http.request(
      new HttpRequest(
        'GET',
        `${this.baseUrl + 'DescargarArchivoCargaObligacion'}`,
        null,
        {
          reportProgress: true,
          responseType: 'blob',
          params,
        }
      )
    );
  }
}
