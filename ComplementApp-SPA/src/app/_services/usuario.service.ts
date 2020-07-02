import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Usuario } from '../_models/usuario';


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
}
