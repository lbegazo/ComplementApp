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
  baseUrl = environment.apiUrl + 'CDP/' ;

  constructor(private http: HttpClient) {}

  ObtenerListaCDP(): Observable<Cdp[]> {
    const path = 'ObtenerListaCDP/';
    return this.http.get<Cdp[]>(this.baseUrl + path);
  }

  ObtenerCDP(numeroCdp: number): Observable<Cdp> {
    const path = 'ObtenerCDP/';
    return this.http.get<Cdp>(this.baseUrl + path + numeroCdp);
  }

  ObtenerDetalleDeCDP(numeroCdp: number): Observable<DetalleCDP[]> {
    const path = 'ObtenerDetalleDeCDP/';
    return this.http.get<DetalleCDP[]>(this.baseUrl + path + numeroCdp);
  }

  RegistrarSolicitudCDP(formato: SolicitudCDP): Observable<any> {
    const path = 'RegistrarSolicitudCDP';
    return this.http.post(this.baseUrl + path, formato);
  }

  ObtenerListaSolicitudCDP(
    solicitudId?: number,
    tipoOperacionId?: number,
    usuarioId?: number,
    fechaRegistro?: Date,
    estadoSolicitudId?: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<SolicitudCDPDto[]>> {
    const path = 'ObtenerListaSolicitudCDP';
    const paginatedResult: PaginatedResult<
    SolicitudCDPDto[]
    > = new PaginatedResult<SolicitudCDPDto[]>();

    let params = new HttpParams();
    if (solicitudId > 0) {
      params = params.append('solicitudId', solicitudId.toString());
    }
    if (tipoOperacionId > 0) {
      params = params.append('tipoOperacionId', tipoOperacionId.toString());
    }
    if (usuarioId > 0) {
      params = params.append('usuarioId', usuarioId.toString());
    }
    if (estadoSolicitudId > 0) {
      params = params.append('estadoSolicitudId', estadoSolicitudId.toString());
    }
    if (fechaRegistro != null) {
      params = params.append('fechaRegistro', fechaRegistro.toString());
    }
    if (page != null) {
      params = params.append('pageNumber', page);
    }
    if (pagesize != null) {
      params = params.append('pageSize', pagesize);
    }

    return this.http
      .get<SolicitudCDPDto[]>(this.baseUrl + path, {
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

  ObtenerSolicitudCDP(solicitudId: number): Observable<SolicitudCDP> {
    const path = 'ObtenerSolicitudCDP';
    let params = new HttpParams();

    if (solicitudId > 0) {
      params = params.append('solicitudId', solicitudId.toString());
    }

    return this.http.get<SolicitudCDP>(this.baseUrl + path, { params });
  }

  
}
