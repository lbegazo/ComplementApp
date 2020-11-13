import { Injectable } from '@angular/core';
import {
  Resolve,
  Router,
  ActivatedRouteSnapshot,
} from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/Operators';
import { Transaccion } from '../_models/transaccion';
import { TransaccionService } from '../_services/transaccion.service';

@Injectable()
export class TransaccionResolver implements Resolve<Transaccion> {
  constructor(
    private userService: TransaccionService,
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
