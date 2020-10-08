import { Injectable } from '@angular/core';
import {
  Resolve,
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/Operators';
import { UsuarioService } from '../_services/usuario.service';
import { Transaccion } from '../_models/transaccion';

@Injectable()
export class TransaccionResolver implements Resolve<Transaccion> {
  constructor(
    private userService: UsuarioService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Transaccion> {
      return this.userService.ObtenerTransaccionXCodigo(route.routeConfig.path).pipe(
      catchError((error) => {
        this.alertify.error('Ocurrió un problema al cargar la transacción');
        this.router.navigate(['/Home']);
        return of(null);
      })
    );
  }
}
