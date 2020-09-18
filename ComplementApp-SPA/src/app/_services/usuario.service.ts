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
  usuarios: Usuario[];

  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
  };

  constructor(private http: HttpClient) {}

  // ObtenerUsuarios(page?, itemPerPage?): Observable<Usuario[]> {
  //   const paginatedResult: PaginatedResult<Usuario[]> = new PaginatedResult<
  //     Usuario[]
  //   >();

  //   let params = new HttpParams();

  //   if (page != null && itemPerPage != null) {
  //     params = params.append('pageNumber', page);
  //     params = params.append('pageSize', itemPerPage);
  //   }

  //   return this.http.get<Usuario[]>(this.baseUrl + 'usuario', { observe: 'response', params})
  //   .pipe(
  //     map(res)
  //   );
  // }

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
    this.http
      .put(this.baseUrl + id, user)
      .pipe(catchError(this.errorHandler))
      .subscribe(
        () => {
          this.ActualizarListaUsuarios();
        },
        (error) => {
          this.errorHandler(error);
        }
      );
    return observableOf(true);
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

  errorHandler(error) {
    let errorMessage = '';

    if (error.error instanceof ErrorEvent) {
      //#region Get client-side error
      errorMessage = error.error.message;
      //console.log('errorHandler client-side error:' + error);

      //#endregion Get client-side error
    } else {
      //#region Get server-side error
      let message = '';
      if (typeof error === 'string' || error instanceof String) {
        message = error as string;
        errorMessage = `Message: ${message}`;
      } else {
        message = error.message;
        errorMessage = `Error Code: ${error.status}\nMessage: ${message}`;
      }

      //console.log('errorHandler server-side error:' + errorMessage);

      //#endregion Get server-side error
    }

    // console.log(errorMessage);
    return throwError(errorMessage);
  }
}
