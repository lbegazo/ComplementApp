import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/Operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { Transaccion } from '../_models/transaccion';
import { Observable, ReplaySubject, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  baseUrl = environment.apiUrl;
  baseUrlPlus = environment.apiUrl + 'auth/';

  jwtHelper = new JwtHelperService();
  decodedToken: any;
  transaccionChanged = new Subject<Transaccion[]>();
  transacciones: Transaccion[] = [];

  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) {}

  login(model: any) {
    return this.http.post(this.baseUrlPlus + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.obtenerListaTransaccionXUsuario1(+this.decodedToken.nameid);
        }
      })
    );
  }

  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }

  private obtenerListaTransaccionXUsuario1(idUsuario: number) {
    this.obtenerListaTransaccionXUsuario(idUsuario).subscribe(
      (lista: Transaccion[]) => {
        this.transacciones = [];
        this.transacciones = lista;
        this.transaccionChanged.next(this.transacciones);
      }
    );
  }

  obtenerListaTransaccionXUsuario(
    idUsuario: number
  ): Observable<Transaccion[]> {
    const path = 'obtenerListaTransaccionXUsuario/';
    return this.http.get<Transaccion[]>(
      this.baseUrl + 'usuario/' + path + idUsuario
    );
  }

  register(user: User) {
    return this.http.post(this.baseUrlPlus + 'register', user);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

  logout() {
    localStorage.clear();
    this.currentUserSource.next(null);
  }

  environmentTest() {
    const token = localStorage.getItem('token');
    this.decodedToken = this.jwtHelper.decodeToken(token);
    const esQA = this.decodedToken?.email !== '' ? true : false;
    return esQA;
  }
}
