import { DatePipe } from '@angular/common';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class GeneralService {
  constructor(public datepipe: DatePipe) {}

  static dateString2Date(dateString: string) {
    const day = +dateString.substr(0, 2);
    const month = +dateString.substr(3, 2) - 1;
    const year = +dateString.substr(6, 4);
    const dateFechaProveedor = new Date(year, month, day);
    return dateFechaProveedor;
  }

  static isValidDate(d) {
    return d instanceof Date;
  }

  static obtenerFormatoMoney(value) {
    if (value || value === 0) {
      value = value.toString().replace('.', ',');
      const valArray = value.toString().split(',');
      for (let i = 0; i < valArray.length; ++i) {
        valArray[i] = valArray[i].replace(/\D/g, '');
      }
      let newVal = '';
      if (valArray.length === 0) {
        newVal = '';
      } else {
        const matches = valArray[0].match(/[0-9]{3}/gim);
        if (matches !== null && valArray[0].length > 3) {
          const commaGroups = Array.from(
            Array.from(valArray[0])
              .reverse()
              .join('')
              .match(/[0-9]{3}/gim)
              .join('.')
          )
            .reverse()
            .join('');
          const replacement = valArray[0].replace(
            commaGroups.replace(/\D/g, ''),
            ''
          );

          newVal =
            (replacement.length > 0 ? replacement + '.' : '') + commaGroups;
        } else {
          newVal = valArray[0];
        }

        if (valArray.length > 1) {
          newVal += ',' + valArray[1].substring(0, 2);
        }
      }
      // set the new value
      return newVal;
    }
  }

  static obtenerFormatoLongMoney(value) {
    if (value || value === 0) {
      value = value.toString().replace('.', ',');
      const valArray = value.toString().split(',');
      for (let i = 0; i < valArray.length; ++i) {
        valArray[i] = valArray[i].replace(/\D/g, '');
      }
      let newVal = '';
      if (valArray.length === 0) {
        newVal = '';
      } else {
        const matches = valArray[0].match(/[0-9]{3}/gim);
        if (matches !== null && valArray[0].length > 3) {
          const commaGroups = Array.from(
            Array.from(valArray[0])
              .reverse()
              .join('')
              .match(/[0-9]{3}/gim)
              .join('.')
          )
            .reverse()
            .join('');
          const replacement = valArray[0].replace(
            commaGroups.replace(/\D/g, ''),
            ''
          );

          newVal =
            (replacement.length > 0 ? replacement + '.' : '') + commaGroups;
        } else {
          newVal = valArray[0];
        }

        if (valArray.length > 1) {
          newVal += ',' + valArray[1].substring(0, 5);
        }
      }
      // set the new value
      return newVal;
    }
  }

  static obtenerValorAbsoluto(value): number {
    if (value) {
      const valor = value.split('.').join('');
      const valor1 = valor.replace(',', '.');
      return valor1;
    }
  }

  convertirAFormatoFecha(d: Date) {
    return this.datepipe.transform(d, 'yyyy-MM-dd');
  }

  // formatMoney(value) {
  //   const temp = `${value}`.replace(/\,/g, '');
  //   return this.currencyPipe
  //     .transform(temp, 'COP', 'symbol', '1.2-2')
  //     .replace('$', '');
  // }

  // this.editForm.valueChanges.subscribe((form) => {
  //   if (form.honorarioSinIvaCtrl) {
  //     this.editForm.patchValue(
  //       {
  //         honorarioSinIvaCtrl: this.currencyPipe.transform(
  //           form.honorarioSinIvaCtrl.replace(/\D/g, '').replace(/^0+/, ''),
  //           'COP', 'symbol', '1.0-0'
  //         ),
  //       },
  //       { emitEvent: false }
  //     );
  //   }
  // });
}
