import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { Cdp } from '../_models/cdp';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { DetalleCDP } from '../_models/detalleCDP';

@Injectable({
  providedIn: 'root',
})
export class CdpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  ObtenerListaCDP(): Observable<Cdp[]> {
    const path = '/ObtenerListaCDP/';
    return this.http.get<Cdp[]>(this.baseUrl + 'CDP' + path);
  }

  ObtenerCDP(numeroCdp: number): Observable<Cdp> {
    const path = '/ObtenerCDP/';
    return this.http.get<Cdp>(this.baseUrl + 'CDP' + path + numeroCdp);
  }

  ObtenerDetalleDeCDP(numeroCdp: number): Observable<DetalleCDP[]> {
    const path = '/ObtenerDetalleDeCDP/';
    return this.http.get<DetalleCDP[]>(
      this.baseUrl + 'CDP' + path + numeroCdp
    );
  }
}
