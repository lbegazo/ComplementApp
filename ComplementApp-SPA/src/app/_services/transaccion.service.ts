import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Transaccion } from '../_models/transaccion';

@Injectable({
  providedIn: 'root',
})
export class TransaccionService {
  baseUrl = environment.apiUrl + 'transaccion/';

  constructor(private http: HttpClient) {}

  ObtenerTransaccionXCodigo(codigo: string): Observable<Transaccion> {
    return this.http.get<Transaccion>(
      this.baseUrl + 'ObtenerTransaccionXCodigo/' + codigo
    );
  }

  obtenerNombreTransaccionPorCodigo(codigo: string) {
    let nombreTransaccion = '';
    let transaccion: Transaccion;
    this.ObtenerTransaccionXCodigo(codigo).subscribe(
      (response: Transaccion) => {
        transaccion = response;
      },
      (error) => {},
      () => {
        if (!transaccion) {
          nombreTransaccion = transaccion.nombre;
        }
        return nombreTransaccion;
      }
    );
    return nombreTransaccion;
  }
}
