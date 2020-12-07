import { DetalleCDP } from './detalleCDP';

export interface Cdp {

  id: number;
  cdpId: number;
  cdp: number;
  fecha: Date;
  detalle1: string; // Estado
  detalle4: string; // Objeto del bien

  crp: number;
  numeroIdentificacionTercero: string;
  nombreTercero: string;

}
