import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, Subject, throwError, of as observableOf } from 'rxjs';
import { map } from 'rxjs/operators';
import { Usuario } from '../_models/usuario';
import { catchError } from 'rxjs/Operators';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { Transaccion } from '../_models/transaccion';
import { ValorSeleccion } from '../_dto/valorSeleccion';

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
    maxSize: 10,
  };

  constructor(private http: HttpClient) {}

  ObtenerUsuariosXFiltro(
    tipo: number,
    usuarioId?: number,
    page?,
    pagesize?
  ): Observable<PaginatedResult<Usuario[]>> {
    const paginatedResult: PaginatedResult<Usuario[]> = new PaginatedResult<
      Usuario[]
    >();

    let params = new HttpParams();
    params = params.append('tipo', tipo.toString());
    if (usuarioId > 0) {
      params = params.append('usuarioId', usuarioId.toString());
    }
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

  ObtenerListaUsuarioXPerfil(perfilId: number): Observable<ValorSeleccion[]> {
    const path = this.baseUrl + 'ObtenerListaUsuarioXPerfil/';
    return this.http.get<ValorSeleccion[]>(path + perfilId);
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

  ObtenerListaUsuario(): Observable<ValorSeleccion[]> {
    const path = this.baseUrl + 'ObtenerListaUsuario/';
    return this.http.get<ValorSeleccion[]>(path);
  }
}
