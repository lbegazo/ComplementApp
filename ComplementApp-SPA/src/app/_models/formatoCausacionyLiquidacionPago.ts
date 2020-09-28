import { Deduccion } from './deduccion';

export interface FormatoCausacionyLiquidacionPago {
  planPagoId: number;
  terceroId: number;
  cantidadPago: number;
  honorario: number;
  honorarioUvt: number;
  valorIva: number;
  valorTotal: number;
  totalRetenciones: number;
  totalAGirar: number;
  baseSalud: number;
  aporteSalud: number;
  aportePension: number;
  riesgoLaboral: number;
  fondoSolidaridad: number;
  impuestoCovid: number;
  subTotal1: number;
  pensionVoluntaria: number;
  afc: number;
  subTotal2: number;
  medicinaPrepagada: number;
  dependientes: number;
  interesVivienda: number;
  totalDeducciones: number;
  subTotal3: number;
  rentaExenta: number;
  limiteRentaExenta: number;
  totalRentaExenta: number;
  diferencialRenta: number;
  baseGravableRenta: number;
  baseGravableUvt: number;
  textoComprobanteContable: string;
  deducciones?: Deduccion[];
}
