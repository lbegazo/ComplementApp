import { Injectable } from '@angular/core';
import {
  Resolve,
  Router,
  ActivatedRouteSnapshot,
} from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/Operators';
import { Area } from '../_models/area';
import { ListaService } from '../_services/lista.service';
import { Cargo } from '../_models/cargo';

@Injectable()
export class ListaCargosResolver implements Resolve<Cargo[]> {
  constructor(
    private listaService: ListaService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Cargo[]> {

    return this.listaService.ObtenerCargos().pipe(
      catchError((error) => {
        this.alertify.error('Ocurrió un problema al cargar la información');
        return of(null);
      })
    );
  }
}
