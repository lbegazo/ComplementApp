import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/Operators';
import { environment } from 'src/environments/environment';
import { ClavePresupuestalContable } from '../_dto/clavePresupuestalContable';
import { Cdp } from '../_models/cdp';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root',
})
export class ObligacionService {
  baseUrl = environment.apiUrl + 'Obligacion';

  constructor(private http: HttpClient) {}

  ObtenerCompromisosParaClavePresupuestalContable(
    terceroId: number,
    numeroCrp: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<Cdp[]>> {
    const paginatedResult: PaginatedResult<Cdp[]> = new PaginatedResult<
      Cdp[]
    >();

    const path = '/ObtenerCompromisosParaClavePresupuestalContable';

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
    const path = '/ObtenerRubrosParaClavePresupuestalContable';

    let params = new HttpParams();

    params = params.append('cdpId', cdpId.toString());

    return this.http.get<ClavePresupuestalContable[]>(this.baseUrl + path, {
      params,
    });
  }

  ObtenerRelacionesContableXRubro(
    rubroPresupuestalId: number
  ): Observable<Cdp[]> {
    const path = '/ObtenerRelacionesContableXRubro';

    let params = new HttpParams();

    params = params.append(
      'rubroPresupuestalId',
      rubroPresupuestalId.toString()
    );

    return this.http.get<Cdp[]>(this.baseUrl + path, { params });
  }
}
