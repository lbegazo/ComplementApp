import { Injectable } from '@angular/core';
import {
  Resolve,
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/Operators';
import { Usuario } from '../_models/usuario';
import { UsuarioService } from '../_services/usuario.service';

@Injectable()
export class UsuarioDetalleResolver implements Resolve<Usuario> {
  constructor(
    private userService: UsuarioService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Usuario> {
    return this.userService.ObtenerUsuario(+route.params['id']).pipe(
      catchError((error) => {
        this.alertify.error('Ocurrió un problema al cargar la información');
        this.router.navigate(['/members']);
        return of(null);
      })
    );
  }
}
