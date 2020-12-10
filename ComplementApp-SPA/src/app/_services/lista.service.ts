import { Injectable } from '@angular/core';
import { Area } from '../_models/area';
import { Cargo } from '../_models/cargo';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { TipoOperacion } from '../_models/tipoOperacion';
import { TipoDetalleCDP } from '../_models/tipoDetalleCDP';
import { Perfil } from '../_models/perfil';
import { ParametroGeneral } from '../_models/parametroGeneral';
import { ParametroLiquidacionTercero } from '../_models/parametroLiquidacionTercero';
import { Estado } from '../_models/estado';
import { ValorSeleccion } from '../_dto/valorSeleccion';
import { UsoPresupuestal } from '../_dto/usoPresupuestal';

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

  ObtenerListaEstadoSolicitudCDP(tipoDocumento: string): Observable<Estado[]> {
    let params = new HttpParams();
    if (tipoDocumento !== '') {
      params = params.append('tipoDocumento', tipoDocumento);
    }

    return this.http.get<Estado[]>(
      this.baseUrl + 'lista/ObtenerListaEstadoSolicitudCDP',
      { params }
    );
  }

  ObtenerListaTipoDetalle(): Observable<TipoDetalleCDP[]> {
    return this.http.get<TipoDetalleCDP[]>(
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

  ObtenerListaMeses(): Observable<ValorSeleccion[]> {
    return this.http.get<ValorSeleccion[]>(
      this.baseUrl + 'lista/ObtenerListaMeses'
    );
  }

  ObtenerParametroGeneralXNombre(nombre: string): Observable<ValorSeleccion> {
    let params = new HttpParams();

    if (nombre.length > 0) {
      params = params.append('nombre', nombre);
    }
    return this.http.get<ValorSeleccion>(
      this.baseUrl + 'lista/ObtenerParametroGeneralXNombre',
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
      this.baseUrl + 'lista/ObtenerListaUsoPresupuestalXRubro',
      { params }
    );
  }

  ObtenerListaXTipo(listaId: number): Observable<ValorSeleccion[]> {
    let params = new HttpParams();

    if (listaId > 0) {
      params = params.append('listaId', listaId.toString());
    }
    return this.http.get<ValorSeleccion[]>(
      this.baseUrl + 'lista/ObtenerListaXTipo',
      { params }
    );
  }

  ObtenerParametrosGeneralesXTipo(tipo: string): Observable<ValorSeleccion[]> {
    let params = new HttpParams();

    params = params.append('tipo', tipo);

    return this.http.get<ValorSeleccion[]>(
      this.baseUrl + 'lista/ObtenerParametrosGeneralesXTipo',
      { params }
    );
  }
}
