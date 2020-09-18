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
import { PlanPago } from '../_models/planPago';
import { PaginatedResult } from '../_models/pagination';
import { PlanPagoService } from '../_services/planPago.service';
import { EstadoPlanPago } from '../_models/enum';

@Injectable()
export class PlanPagoResolver implements Resolve<PlanPago[]> {
  pageNumber = 1;
  pageSize = 5;
  estadoPlanPagoPorObligar = EstadoPlanPago.PorObligar.value;

  constructor(
    private planPagoService: PlanPagoService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<PlanPago[]> {
    return this.planPagoService
      .ObtenerListaPlanPago(
        this.estadoPlanPagoPorObligar.toString(),
        null,
        this.pageNumber,
        this.pageSize
      )
      .pipe(
        catchError((error) => {
          this.alertify.error(
            'Ocurrió un problema al cargar los planes de pago'
          );
          this.router.navigate(['/causacion']);
          return of(null);
        })
      );
  }
}
