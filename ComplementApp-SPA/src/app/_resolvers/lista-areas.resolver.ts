import { Injectable } from '@angular/core';
import {
  Resolve,
  ActivatedRouteSnapshot,
} from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/Operators';
import { Area } from '../_models/area';
import { ListaService } from '../_services/lista.service';

@Injectable()
export class ListaAreasResolver implements Resolve<Area[]> {
  constructor(
    private listaService: ListaService,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Area[]> {

    return this.listaService.ObtenerAreas().pipe(
      catchError((error) => {
        this.alertify.error('Ocurrió un problema al cargar la información');
        return of(null);
      })
    );
  }
}
