import { Injectable } from '@angular/core';
import {
  Resolve,
  Router,
  ActivatedRouteSnapshot,
} from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/Operators';
import { ListaService } from '../_services/lista.service';
import { Perfil } from '../_models/perfil';

@Injectable()
export class ListaPerfilesResolver implements Resolve<Perfil[]> {
  constructor(
    private listaService: ListaService,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Perfil[]> {

    return this.listaService.ObtenerListaPerfiles().pipe(
      catchError((error) => {
        this.alertify.error('Ocurrió un problema al cargar la información');
        return of(null);
      })
    );
  }
}
