import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { Cdp } from '../_models/cdp';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { DetalleCDP } from '../_models/detalleCDP';
import { SolicitudCDP } from '../_models/solicitudCDP';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/Operators';
import { SolicitudCDPDto } from '../_models/solicitudCDPDto';

@Injectable({
  providedIn: 'root',
})
export class CdpService {
  baseUrl = environment.apiUrl + 'CDP/';

  constructor(private http: HttpClient) {}

  ObtenerListaCompromiso(page?, pagesize?): Observable<PaginatedResult<Cdp[]>> {
    let params = new HttpParams();
    const path = 'ObtenerListaCompromiso';
    const paginatedResult: PaginatedResult<Cdp[]> = new PaginatedResult<
      Cdp[]
    >();

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

  

  ObtenerRubrosPresupuestalesPorCompromiso(
    crp: number
  ): Observable<DetalleCDP[]> {
    const path = 'ObtenerRubrosPresupuestalesPorCompromiso/';
    return this.http.get<DetalleCDP[]>(this.baseUrl + path + crp);
  }
}
