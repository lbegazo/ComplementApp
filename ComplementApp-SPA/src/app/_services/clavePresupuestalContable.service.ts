import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/Operators';
import { environment } from 'src/environments/environment';
import { ClavePresupuestalContableDto } from '../_dto/clavePresupuestalContableDto';
import { RelacionContableDto } from '../_dto/relacionContableDto';
import { ValorSeleccion } from '../_dto/valorSeleccion';
import { Cdp } from '../_models/cdp';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root',
})
export class ClavePresupuestalContableService {
  baseUrl = environment.apiUrl + 'ClavePresupuestalContable/';

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

  ObtenerRubrosPresupuestalesXCompromiso(
    crp: number
  ): Observable<ClavePresupuestalContableDto[]> {
    const path = 'ObtenerRubrosPresupuestalesXCompromiso';

    let params = new HttpParams();

    params = params.append('crp', crp.toString());

    return this.http.get<ClavePresupuestalContableDto[]>(this.baseUrl + path, {
      params,
    });
  }

  ObtenerRelacionesContableXRubroPresupuestal(
    rubroPresupuestalId: number
  ): Observable<RelacionContableDto[]> {
    const path = 'ObtenerRelacionesContableXRubroPresupuestal';

    let params = new HttpParams();

    params = params.append(
      'rubroPresupuestalId',
      rubroPresupuestalId.toString()
    );

    return this.http.get<RelacionContableDto[]>(this.baseUrl + path, {
      params,
    });
  }

  ObtenerUsosPresupuestalesXRubroPresupuestal(
    rubroPresupuestalId: number
  ): Observable<ValorSeleccion[]> {
    const path = 'ObtenerUsosPresupuestalesXRubroPresupuestal';

    let params = new HttpParams();

    params = params.append(
      'rubroPresupuestalId',
      rubroPresupuestalId.toString()
    );

    return this.http.get<ValorSeleccion[]>(this.baseUrl + path, { params });
  }

  RegistrarClavePresupuestalContable(
    lista: ClavePresupuestalContableDto[]
  ): Observable<any> {
    const path = 'RegistrarClavePresupuestalContable';
    return this.http.post(this.baseUrl + path, lista);
  }
}
