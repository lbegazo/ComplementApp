import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { ActividadGeneralPrincipalDto } from '../_dto/actividadGeneralPrincipalDto';
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
}
