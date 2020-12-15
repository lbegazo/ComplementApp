import { Cdp } from '../_models/cdp';
import { Contrato } from '../_models/contrato';
import { Tercero } from '../_models/tercero';

export interface FormatoSolicitudPagoDto {
  formatoSolicitudPagoId: number;
  cdp: Cdp;
  tercero: Tercero;
  contrato: Contrato;
  valorPagadoFechaActual: number;
  numeroPagoFechaActual: number;
  cantidadMaxima: number;
  pagosRealizados: Cdp[];
}
