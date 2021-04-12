import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { ActividadGeneralPrincipalDto } from '../_dto/actividadGeneralPrincipalDto';
import { ActividadEspecifica } from '../_models/actividadEspecifica';
import { ActividadGeneral } from '../_models/actividadGeneral';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root',
})
export class ActividadGeneralService {
  baseUrl = environment.apiUrl + 'actividadGeneral/';

  constructor(private http: HttpClient) {}

  ObtenerActividadesGenerales(): Observable<ActividadGeneral[]> {
    const path = 'ObtenerActividadesGenerales';
    return this.http.get<ActividadGeneral[]>(this.baseUrl + path, {});
  }

  RegistrarActividadesGenerales(
    principal: ActividadGeneralPrincipalDto
  ): Observable<any> {
    const path = 'RegistrarActividadesGenerales';
    return this.http.post(this.baseUrl + path, principal);
  }

  ObtenerActividadesEspecificas(): Observable<ActividadEspecifica[]> {
    const path = 'ObtenerActividadesEspecificas';
    return this.http.get<ActividadEspecifica[]>(this.baseUrl + path, {});
  }

  ObtenerListaActividadEspecifica(
    page?,
    pagesize?
  ): Observable<PaginatedResult<ActividadEspecifica[]>> {
    let params = new HttpParams();
    const path = 'ObtenerListaActividadEspecifica';
    const paginatedResult: PaginatedResult<
      ActividadEspecifica[]
    > = new PaginatedResult<ActividadEspecifica[]>();

    if (page != null) {
      params = params.append('pageNumber', page);
    }
    if (pagesize != null) {
      params = params.append('pageSize', pagesize);
    }

    return this.http
      .get<ActividadEspecifica[]>(this.baseUrl + path, {
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

  RegistrarActividadesEspecificas(
    actividadEspecifica: ActividadEspecifica
  ): Observable<any> {
    const path = 'RegistrarActividadesEspecificas';
    return this.http.post(this.baseUrl + path, actividadEspecifica);
  }
}
