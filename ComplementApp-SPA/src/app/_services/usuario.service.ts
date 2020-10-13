import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, Subject, throwError, of as observableOf } from 'rxjs';
import { map } from 'rxjs/operators';
import { Usuario } from '../_models/usuario';
import { catchError } from 'rxjs/Operators';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { Transaccion } from '../_models/transaccion';

@Injectable({
  providedIn: 'root',
})
export class UsuarioService {
  baseUrl = environment.apiUrl + 'usuario/';
  usuarioChanged = new Subject<Usuario[]>();
  usuarios: Usuario[] = [];

  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
  };

  constructor(private http: HttpClient) {}

  ObtenerUsuarios(page?, pagesize?): Observable<PaginatedResult<Usuario[]>> {
    const paginatedResult: PaginatedResult<Usuario[]> = new PaginatedResult<
      Usuario[]
    >();

    let params = new HttpParams();

    if (page != null) {
      params = params.append('pageNumber', page);
    }
    if (pagesize != null) {
      params = params.append('pageSize', pagesize);
    }

    return this.http
      .get<Usuario[]>(this.baseUrl, {
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

  ObtenerUsuario(id: number): Observable<Usuario> {
    return this.http.get<Usuario>(this.baseUrl + id);
  }

  ActualizarUsuario(id: number, user: Usuario): Observable<boolean> {
    this.http.put(this.baseUrl + id, user).subscribe(() => {
      this.ActualizarListaUsuarios();
      return observableOf(true);
    });
    return observableOf(false);
  }

  EliminarUsuario(id: number) {
    return this.http.delete(this.baseUrl + id).pipe(
      map((response: any) => {
        this.ActualizarListaUsuarios();
      })
    );
  }

  RegistrarUsuario(user: Usuario) {
    return this.http.post(this.baseUrl, user).pipe(
      map((response: any) => {
        this.ActualizarListaUsuarios();
      })
    );
  }

  private ActualizarListaUsuarios() {
    this.ObtenerUsuarios(
      this.pagination.currentPage,
      this.pagination.itemsPerPage
    ).subscribe((response: PaginatedResult<Usuario[]>) => {
      this.usuarios = response.result;
      this.usuarioChanged.next(this.usuarios);
    });
  }

  ObtenerTransaccionXCodigo(codigo: string): Observable<Transaccion> {
    return this.http.get<Transaccion>(
      this.baseUrl + 'ObtenerTransaccionXCodigo/' + codigo
    );
  }

  obtenerNombreTransaccionPorCodigo(codigo: string) {
    let nombreTransaccion = '';
    let transaccion: Transaccion;
    this.ObtenerTransaccionXCodigo(codigo).subscribe(
      (response: Transaccion) => {
        transaccion = response;
      },
      (error) => {},
      () => {
        if (!transaccion) {
          nombreTransaccion = transaccion.nombre;
        }
        return nombreTransaccion;
      }
    );
    return nombreTransaccion;
  }
}
