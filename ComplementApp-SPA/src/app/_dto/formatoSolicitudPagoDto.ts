import { Cdp } from '../_models/cdp';
import { Contrato } from '../_models/contrato';
import { PlanPago } from '../_models/planPago';
import { Tercero } from '../_models/tercero';
import { DetalleFormatoSolicitudPagoDto } from './detalleFormatoSolicitudPagoDto';
import { ValorSeleccion } from './valorSeleccion';

export interface FormatoSolicitudPagoDto {
  formatoSolicitudPagoId: number;
  planPagoId: number;
  fechaInicio: Date;
  fechaFinal: Date;
  fechaSistema: Date;
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

  valorPagadoFechaActual: number;
  numeroPagoFechaActual: number;
  cantidadMaxima: number;
  aporteSalud: number;
  aportePension: number;
  riesgoLaboral: number;
  fondoSolidaridad: number;
  estadoId: number;
  numeroRadicadoProveedor: string;
  fechaRadicadoProveedor: Date;
  numeroRadicadoSupervisor: string;
  fechaRadicadoSupervisor: Date;
  tipoAdminPila: string;

  actividadEconomica: ValorSeleccion;
  planPago: PlanPago;
  pagosRealizados: Cdp[];
  cdp: Cdp;
  tercero: Tercero;
  contrato: Contrato;
  detallesFormatoSolicitudPago: DetalleFormatoSolicitudPagoDto[];
}
