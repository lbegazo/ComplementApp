import { DetalleFormatoSolicitudPagoDto } from '../_dto/detalleFormatoSolicitudPagoDto';

export interface FormatoSolicitudPago {
  formatoSolicitudPagoId: number;
  terceroId: number;
  planPagoId: number;
  crp: number;
  numeroFactura: string;
  valorFacturado: number;
  actividadEconomicaId: number;
  actividadEconomicaDescripcion: string;
  fechaInicio: Date;
  fechaFinal: Date;
  valorBaseGravableRenta: number;
  valorIva: number;
  observaciones: string;
  numeroPlanilla: string;
  mesId: number;
  mes: string;
  baseCotizacion: number;
  supervisorId: number;
  detallesFormatoSolicitudPago: DetalleFormatoSolicitudPagoDto[];
}
