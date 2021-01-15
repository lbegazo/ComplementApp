import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/Operators';
import { ValorSeleccion } from '../_dto/valorSeleccion';
import { PerfilUsuario } from '../_models/enum';
import { UsuarioService } from '../_services/usuario.service';

@Injectable()
export class ListaSupervisorResolver
  implements Resolve<ValorSeleccion[]> {
  constructor(
    private usuarioService: UsuarioService,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<ValorSeleccion[]> {
    return this.usuarioService
      .ObtenerListaUsuarioXPerfil(PerfilUsuario.SupervisorContractual.value)
      .pipe(
        catchError((error) => {
          this.alertify.error('Ocurrió un problema al cargar la información');
          return of(null);
        })
      );
  }
}
