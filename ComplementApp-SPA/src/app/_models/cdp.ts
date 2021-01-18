import { DetalleCDP } from './detalleCDP';

export interface Cdp {
  id: number;
  cdpId: number;
  cdp: number;
  crp: number;
  ordenPago: number;

  fecha: Date;
  detalle1: string; // Estado
  detalle4: string; // Objeto del bien
  objeto: string; // Objeto del bien completo
  detalle5: string; // Supervisor
  supervisorId: number;

  valorInicial: number;
  operacion: number;
  valorTotal: number;
  saldoActual: number;

  numeroIdentificacionTercero: string;
  nombreTercero: string;

  formatoSolicitudPagoId: number;
}
