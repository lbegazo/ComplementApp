import { Injectable } from '@angular/core';
import { Area } from '../_models/area';
import { Cargo } from '../_models/cargo';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { TipoOperacion } from '../_models/tipoOperacion';
import { TipoDetalleCDP } from '../_models/tipoDetalleCDP';
import { Perfil } from '../_models/perfil';
import { Estado } from '../_models/estado';
import { ValorSeleccion } from '../_dto/valorSeleccion';
import { UsoPresupuestal } from '../_dto/usoPresupuestal';
import { PaginatedResult } from '../_models/pagination';
import { RubroPresupuestal } from '../_models/rubroPresupuestal';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ListaService {
  baseUrl = environment.apiUrl + 'lista/';

  constructor(private http: HttpClient) {}

  //#region CDP

  ObtenerAreas(): Observable<Area[]> {
    return this.http.get<Area[]>(this.baseUrl + 'ObtenerAreas');
  }

  ObtenerCargos(): Observable<Cargo[]> {
    return this.http.get<Cargo[]>(this.baseUrl + 'ObtenerCargos');
  }

  ObtenerListaTipoOperacion(): Observable<TipoOperacion[]> {
    return this.http.get<TipoOperacion[]>(
      this.baseUrl + 'ObtenerListaTipoOperacion'
    );
  }

  ObtenerListaEstadoSolicitudCDP(tipoDocumento: string): Observable<Estado[]> {
    let params = new HttpParams();
    if (tipoDocumento !== '') {
      params = params.append('tipoDocumento', tipoDocumento);
    }

    return this.http.get<Estado[]>(
      this.baseUrl + 'ObtenerListaEstadoSolicitudCDP',
      { params }
    );
  }

  ObtenerListaTipoDetalle(): Observable<TipoDetalleCDP[]> {
    return this.http.get<TipoDetalleCDP[]>(
      this.baseUrl + 'ObtenerListaTipoDetalleModificacion'
    );
  }

  //#endregion CDP

  //#region Transaccion

  obtenerNombreTransaccionPorCodigo(codigoTransaccion: string) {
    let nombreDescripcion = '';
    const storageVal = localStorage.getItem('Transacciones');
    const transacciones = storageVal ? JSON.parse(storageVal) : [];
    const transaccion = transacciones.filter(
      (x) => x.codigo === codigoTransaccion
    )[0];

    if (transaccion) {
      nombreDescripcion = transaccion.descripcion;
    }

    return nombreDescripcion;
  }

  ObtenerListaPerfiles(): Observable<Perfil[]> {
    return this.http.get<Perfil[]>(this.baseUrl + 'ObtenerListaPerfiles');
  }

  //#endregion

  ObtenerListaMeses(): Observable<ValorSeleccion[]> {
    return this.http.get<ValorSeleccion[]>(
      this.baseUrl + 'ObtenerListaMeses'
    );
  }

  ObtenerParametroGeneralXNombre(nombre: string): Observable<ValorSeleccion> {
    let params = new HttpParams();

    if (nombre.length > 0) {
      params = params.append('nombre', nombre);
    }
    return this.http.get<ValorSeleccion>(
      this.baseUrl + 'ObtenerParametroGeneralXNombre',
      { params }
    );
  }

  ObtenerListaUsoPresupuestalXRubro(
    rubroPresupuestalId: number
  ): Observable<UsoPresupuestal[]> {
    let params = new HttpParams();

    if (rubroPresupuestalId > 0) {
      params = params.append(
        'rubroPresupuestalId',
        rubroPresupuestalId.toString()
      );
    }
    return this.http.get<UsoPresupuestal[]>(
      this.baseUrl + 'ObtenerListaUsoPresupuestalXRubro',
      { params }
    );
  }

  ObtenerListaXTipo(listaId: number): Observable<ValorSeleccion[]> {
    let params = new HttpParams();

    if (listaId > 0) {
      params = params.append('listaId', listaId.toString());
    }
    return this.http.get<ValorSeleccion[]>(
      this.baseUrl + 'ObtenerListaXTipo',
      { params }
    );
  }

  ObtenerListaXTipoyPci(listaId: number): Observable<ValorSeleccion[]> {
    let params = new HttpParams();

    if (listaId > 0) {
      params = params.append('listaId', listaId.toString());
    }
    return this.http.get<ValorSeleccion[]>(
      this.baseUrl + 'ObtenerListaXTipoyPci',
      { params }
    );
  }

  ObtenerListaSIoNO(): Observable<ValorSeleccion[]> {
    return this.http.get<ValorSeleccion[]>(
      this.baseUrl + 'ObtenerListaSIoNO'
    );
  }

  ObtenerParametrosGeneralesXTipo(tipo: string): Observable<ValorSeleccion[]> {
    let params = new HttpParams();

    params = params.append('tipo', tipo);

    return this.http.get<ValorSeleccion[]>(
      this.baseUrl + 'ObtenerParametrosGeneralesXTipo',
      { params }
    );
  }

  ObtenerListaRubroPresupuestalPorPapa(
    rubroPresupuestalId,
    page?,
    pagesize?
  ): Observable<PaginatedResult<RubroPresupuestal[]>> {
    let params = new HttpParams();
    const path = 'ObtenerListaRubroPresupuestalPorPapa';
    const paginatedResult: PaginatedResult<
      RubroPresupuestal[]
    > = new PaginatedResult<RubroPresupuestal[]>();

    if (rubroPresupuestalId != null) {
      params = params.append('rubroPresupuestalId', rubroPresupuestalId);
    }
    if (page != null) {
      params = params.append('pageNumber', page);
    }
    if (pagesize != null) {
      params = params.append('pageSize', pagesize);
    }

    return this.http
      .get<RubroPresupuestal[]>(this.baseUrl + path, {
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
}
