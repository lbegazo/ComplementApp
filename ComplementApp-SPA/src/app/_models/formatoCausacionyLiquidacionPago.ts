import { Deduccion } from './deduccion';

export interface FormatoCausacionyLiquidacionPago {
  planPagoId: number;
  detalleLiquidacionId: number;
  terceroId: number;

  identificacionTercero: string;
  nombreTercero: string;
  contrato: string;
  viaticos: boolean;
  viaticosDescripcion: string;
  crp: string;
  cantidadPago: number;
  numeroPago: number;
  modalidadContrato: number;
  valorContrato: number;
  valorAdicionReduccion: number;
  valorCancelado: number;
  totalACancelar: number;
  saldoActual: number;
  identificacionRubroPresupuestal: string;
  identificacionUsoPresupuestal: string;
  nombreSupervisor: string;
  numeroRadicadoSupervisor: string;
  fechaRadicadoSupervisor: Date;
  numeroFactura: string;
  textoComprobanteContable: string;
  viaticosPagados: number;

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

  deducciones?: Deduccion[];
}
