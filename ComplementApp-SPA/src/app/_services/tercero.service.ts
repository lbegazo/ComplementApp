import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { map } from 'rxjs/Operators';
import { environment } from 'src/environments/environment';
import { TerceroDeduccionDto } from '../_dto/terceroDeduccionDto';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { ParametroLiquidacionTercero } from '../_models/parametroLiquidacionTercero';
import { Tercero } from '../_models/tercero';

@Injectable({
  providedIn: 'root',
})
export class TerceroService {
  baseUrl = environment.apiUrl + 'tercero/';
  deducciones: TerceroDeduccionDto[];

  constructor(private http: HttpClient) {}

  //#region Tercero

  ObtenerTerceros(
    tipo: number,
    terceroId?: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<Tercero[]>> {
    const path = 'ObtenerTerceros';
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

  ObtenerTercero(terceroId: number): Observable<Tercero> {
    const path = 'ObtenerTercero';
    let params = new HttpParams();
    params = params.append('terceroId', terceroId.toString());
    return this.http.get<Tercero>(this.baseUrl + path, {
      params,
    });
  }

  ActualizarTercero(tercero: Tercero) {
    const path = 'ActualizarTercero';
    return this.http.put(this.baseUrl + path, tercero);
  }

  RegistrarTercero(tercero: Tercero): Observable<any> {
    const path = 'RegistrarTercero';
    return this.http.post(this.baseUrl + path, tercero);
  }

  //#endregion Tercero

  //#region Parametro Liquidacion Tercero

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

  ObtenerDeduccionesXTercero2(terceroId: number): TerceroDeduccionDto[] {
    const path = 'ObtenerDeduccionesXTercero';
    let params = new HttpParams();
    params = params.append('terceroId', terceroId.toString());
    this.http
      .get<TerceroDeduccionDto[]>(this.baseUrl + path, {
        params,
      })
      .subscribe((lista: TerceroDeduccionDto[]) => {
        this.deducciones = lista;
      });

    return this.deducciones;
  }

  ObtenerDeduccionesXTercero(
    terceroId: number
  ): Observable<TerceroDeduccionDto[]> {
    const path = 'ObtenerDeduccionesXTercero';
    let params = new HttpParams();
    params = params.append('terceroId', terceroId.toString());
    return this.http.get<TerceroDeduccionDto[]>(this.baseUrl + path, {
      params,
    });
  }

  AddDeducciones(deducion: TerceroDeduccionDto) {
    this.deducciones.push(deducion);
  }

  ObteneListaDeducciones(): Observable<TerceroDeduccionDto[]> {
    const path = 'ObteneListaDeducciones';
    return this.http.get<TerceroDeduccionDto[]>(this.baseUrl + path);
  }

  //#endregion Parametro Liquidacion Tercero
}
