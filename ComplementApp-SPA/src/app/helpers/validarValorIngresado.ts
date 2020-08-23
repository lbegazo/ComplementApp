import { ValidatorFn, AbstractControl } from '@angular/forms';

export class ValidarValorIngresado {
  static valorIncorrecto(
    idTipoOperacion: number,
    saldoAct: number,
    valorCDP: number
  ): ValidatorFn {
    return (c: AbstractControl): { [key: string]: boolean } | null => {
      if (idTipoOperacion === 4 || idTipoOperacion === 3) {
        if (
          (c.value || c.value === 0) &&
          (isNaN(c.value) || c.value > saldoAct)
        ) {
          // Solicitud inicial o adición
          return { valorIncorrecto: true };
        }
      }
      if (idTipoOperacion === 2) {
        if (
          (c.value || c.value === 0) &&
          (isNaN(c.value) || c.value > valorCDP)
        ) {
          // Reducción
          return { valorIncorrecto: true };
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
