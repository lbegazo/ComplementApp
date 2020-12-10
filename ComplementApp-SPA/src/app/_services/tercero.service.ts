import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { map } from 'rxjs/Operators';
import { environment } from 'src/environments/environment';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { ParametroLiquidacionTercero } from '../_models/parametroLiquidacionTercero';
import { Tercero } from '../_models/tercero';

@Injectable({
  providedIn: 'root',
})
export class TerceroService {
  baseUrl = environment.apiUrl + 'tercero/';

  constructor(private http: HttpClient) {}

  ObtenerTercerosParaParametrizacionLiquidacion(
    tipo: number,
    terceroId?: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<Tercero[]>> {
    const path = 'ObtenerTercerosParaParametrizacionLiquidacion';
    const paginatedResult: PaginatedResult<Tercero[]> = new PaginatedResult<
      Tercero[]
    >();

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
      .get<Tercero[]>(this.baseUrl + path, {
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

  ObtenerParametrizacionLiquidacionXTercero(
    terceroId: number
  ): Observable<ParametroLiquidacionTercero> {
    const path = 'ObtenerParametrizacionLiquidacionXTercero';
    let params = new HttpParams();
    params = params.append('terceroId', terceroId.toString());
    return this.http.get<ParametroLiquidacionTercero>(this.baseUrl + path, {
      params,
    });
  }

  ActualizarParametroLiquidacionTercero(user: ParametroLiquidacionTercero) {
    const path = 'ActualizarParametroLiquidacionTercero';
    return this.http.put(this.baseUrl + path, user);
  }

  RegistrarParametroLiquidacionTercero(user: ParametroLiquidacionTercero) {
    const path = 'RegistrarParametroLiquidacionTercero';
    return this.http.post(this.baseUrl + path, user);
  }
}
