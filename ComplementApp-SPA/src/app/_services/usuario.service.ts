import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Usuario } from '../_models/usuario';
import { Area } from '../_models/area';
import { Cargo } from '../_models/cargo';


@Injectable({
  providedIn: 'root',
})
export class UsuarioService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  ObtenerUsuarios(): Observable<Usuario[]> {
    return this.http.get<Usuario[]>(this.baseUrl + 'usuario');
  }

  ObtenerUsuario(id: number): Observable<Usuario> {
    return this.http.get<Usuario>(this.baseUrl + 'usuario/' + id);
  }

  ActualizarUsuario(id: number, user: Usuario) {
    return this.http.put(this.baseUrl + 'usuario/' + id, user);
  }

  RegistrarUsuario(user: Usuario) {
    return this.http.post(this.baseUrl + 'usuario/', user);
  }

  ObtenerAreas(): Observable<Area[]> {
    return this.http.get<Area[]>(this.baseUrl + 'usuario/ObtenerAreas');
  }

  ObtenerCargos(): Observable<Cargo[]> {
    return this.http.get<Cargo[]>(this.baseUrl + 'usuario/ObtenerCargos');
  }
}
