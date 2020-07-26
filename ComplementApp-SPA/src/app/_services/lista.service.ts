import { Injectable } from '@angular/core';
import { Area } from '../_models/area';
import { Cargo } from '../_models/cargo';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { TipoOperacion } from '../_models/tipoOperacion';
import { TipoDetalle } from '../_models/tipoDetalle';

@Injectable({
  providedIn: 'root'
})
export class ListaService 
{
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  ObtenerAreas(): Observable<Area[]> {
    return this.http.get<Area[]>(this.baseUrl + 'lista/ObtenerAreas');
  }

  ObtenerCargos(): Observable<Cargo[]> {
    return this.http.get<Cargo[]>(this.baseUrl + 'lista/ObtenerCargos');
  }

  ObtenerListaTipoOperacion(): Observable<TipoOperacion[]> {
    return this.http.get<TipoOperacion[]>(this.baseUrl + 'lista/ObtenerListaTipoOperacion');
  }

  ObtenerListaTipoDetalle(): Observable<TipoDetalle[]> {
    return this.http.get<TipoOperacion[]>(this.baseUrl + 'lista/ObtenerListaTipoDetalleModificacion');
  }


}
