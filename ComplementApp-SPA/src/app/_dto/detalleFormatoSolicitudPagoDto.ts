import { ValorSeleccion } from './valorSeleccion';

export interface DetalleFormatoSolicitudPagoDto {
  detalleFormatoSolicitudPagoId: number;
  formatoSolicitudPagoId: number;
  rubroPresupuestal: ValorSeleccion;
  valorAPagar: number;
  dependencia: string;
}
