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
import { Cdp } from '../_models/cdp';
import { CdpService } from '../_services/cdp.service';

@Injectable()
export class CdpDetalleResolver implements Resolve<Cdp> {
  constructor(
    private cdpService: CdpService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Cdp>
  {
      var cdp = this.cdpService.ObtenerCDP(+route.params['id']);
      
    return this.cdpService.ObtenerCDP(+route.params['id']).pipe(
      catchError((error) => {
        this.alertify.error('Ocurrió un problema al cargar la información');
        this.router.navigate(['/cdp']);
        return of(null);
      })
    );
  }
}
