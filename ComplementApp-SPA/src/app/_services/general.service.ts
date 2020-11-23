import { DatePipe } from '@angular/common';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class GeneralService {
  constructor(public datepipe: DatePipe) {}

  convertirAFormatoFecha(d: Date) {
    return this.datepipe.transform(d, 'yyyy-MM-dd');
  }

  dateString2Date(dateString: string) {
    const day = +dateString.substr(0, 2);
    const month = +dateString.substr(3, 2) - 1;
    const year = +dateString.substr(6, 4);
    const dateFechaProveedor = new Date(year, month, day);
    return dateFechaProveedor;
  }

  isValidDate(d) {
    return d instanceof Date;
  }
}
