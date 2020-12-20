import { Cdp } from '../_models/cdp';
import { Contrato } from '../_models/contrato';
import { PlanPago } from '../_models/planPago';
import { Tercero } from '../_models/tercero';
import { ValorSeleccion } from './valorSeleccion';

export interface FormatoSolicitudPagoDto {
  formatoSolicitudPagoId: number;
  planPagoId: number;
  fechaInicio: Date;
  fechaFinal: Date;
  valorFacturado: number;
  mesId: number;
  mesDescripcion: string;
  numeroPlanilla: string;
  numeroFactura: string;
  observaciones: string;
  valorBaseGravableRenta: number;
  valorIva: number;
  baseCotizacion: number;
  observacionesModificacion: string;
  cdp: Cdp;
  tercero: Tercero;
  contrato: Contrato;
  actividadEconomica: ValorSeleccion;
  planPago: PlanPago;
  valorPagadoFechaActual: number;
  numeroPagoFechaActual: number;
  cantidadMaxima: number;
  aporteSalud: number;
  aportePension: number;
  riesgoLaboral: number;
  fondoSolidaridad: number;
  pagosRealizados: Cdp[];
  estadoId: number;
}
