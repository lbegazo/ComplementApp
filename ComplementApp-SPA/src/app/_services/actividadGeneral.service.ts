import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ActividadGeneralPrincipalDto } from '../_dto/actividadGeneralPrincipalDto';
import { ActividadEspecifica } from '../_models/actividadEspecifica';
import { ActividadGeneral } from '../_models/actividadGeneral';

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

  RegistrarActividadesEspecificas(
    principal: ActividadGeneralPrincipalDto
  ): Observable<any> {
    const path = 'RegistrarActividadesEspecificas';
    return this.http.post(this.baseUrl + path, principal);
  }
}
