import { ValidatorFn, AbstractControl } from '@angular/forms';
import { GeneralService } from '../_services/general.service';

export class ValidarValorIngresado {
  /**
   Anulación 1
   Solicitud Inicial 4
   Reducción 2
   Adición 3
   **/
  constructor() {}

  static valorIncorrecto(
    idTipoOperacion: number,
    saldoAct: number,
    valorCDP: number
  ): ValidatorFn {
    let valor = 0;

    return (c: AbstractControl): { [key: string]: boolean } | null => {
      if (c.value || c.value === 0) {
        valor = GeneralService.obtenerValorAbsoluto(c.value);
        if (idTipoOperacion === 4 || idTipoOperacion === 3) {
          if ((valor || valor === 0) && (isNaN(valor) || valor > saldoAct)) {
            // Solicitud inicial o adición
            return { valorIncorrecto: true };
          }
        }
        if (idTipoOperacion === 2) {
          if ((valor || valor === 0) && (isNaN(valor) || valor > valorCDP)) {
            // Reducción
            return { valorIncorrecto: true };
          }
        }
      }
      return null;
    };
  }

  static range(min: number, max: number): ValidatorFn {
    return (c: AbstractControl): { [key: string]: boolean } | null => {
      if (
        (c.value || c.value === 0) &&
        (isNaN(c.value) || c.value < min || c.value > max)
      ) {
        return { range: true };
      }
      return null;
    };
  }
}
