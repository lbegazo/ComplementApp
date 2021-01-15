import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/Operators';
import { ListaService } from '../_services/lista.service';
import { ValorSeleccion } from '../_dto/valorSeleccion';
import { TipoLista } from '../_models/enum';

@Injectable()
export class ListaModalidadContratoResolver
  implements Resolve<ValorSeleccion[]> {
  constructor(
    private listaService: ListaService,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<ValorSeleccion[]> {
    return this.listaService
      .ObtenerListaXTipo(TipoLista.ModalidadContrato.value)
      .pipe(
        catchError((error) => {
          this.alertify.error('Ocurrió un problema al cargar la información');
          return of(null);
        })
      );
  }
}
