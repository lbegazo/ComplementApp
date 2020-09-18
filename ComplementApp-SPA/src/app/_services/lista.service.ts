import { Injectable } from '@angular/core';
import { Area } from '../_models/area';
import { Cargo } from '../_models/cargo';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { TipoOperacion } from '../_models/tipoOperacion';
import { TipoDetalle } from '../_models/tipoDetalle';
import { Perfil } from '../_models/perfil';
import { ParametroGeneral } from '../_models/parametroGeneral';
import { ParametroLiquidacionTercero } from '../_models/parametroLiquidacionTercero';

@Injectable({
  providedIn: 'root',
})
export class ListaService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  //#region CDP

  ObtenerAreas(): Observable<Area[]> {
    return this.http.get<Area[]>(this.baseUrl + 'lista/ObtenerAreas');
  }

  ObtenerCargos(): Observable<Cargo[]> {
    return this.http.get<Cargo[]>(this.baseUrl + 'lista/ObtenerCargos');
  }

  ObtenerListaTipoOperacion(): Observable<TipoOperacion[]> {
    return this.http.get<TipoOperacion[]>(
      this.baseUrl + 'lista/ObtenerListaTipoOperacion'
    );
  }

  ObtenerListaTipoDetalle(): Observable<TipoDetalle[]> {
    return this.http.get<TipoDetalle[]>(
      this.baseUrl + 'lista/ObtenerListaTipoDetalleModificacion'
    );
  }

  //#endregion CDP

  //#region Liquidación Factura

  ObtenerParametrosGenerales(): Observable<ParametroGeneral[]> {
    return this.http.get<ParametroGeneral[]>(
      this.baseUrl + 'lista/ObtenerParametrosGenerales'
    );
  }

  ObtenerParametroLiquidacionXTercero(
    terceroId: number
  ): Observable<ParametroLiquidacionTercero> {
    let params = new HttpParams();

    if (terceroId > 0) {
      params = params.append('terceroId', terceroId.toString());
    }

    return this.http.get<ParametroLiquidacionTercero>(
      this.baseUrl + 'lista/ObtenerParametroLiquidacionXTercero',
      { params }
    );
  }

  //#endregion Liquidación Factura

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
    return this.http.get<Perfil[]>(this.baseUrl + 'lista/ObtenerListaPerfiles');
  }

  //#endregion
}
