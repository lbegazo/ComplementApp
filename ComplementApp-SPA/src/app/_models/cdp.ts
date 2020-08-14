import { DetalleCDP } from './detalleCDP';

export interface Cdp {

  id: number;
  cdp: number;
  fecha: Date;
  detalle1: string; // Estado
  detalle4: string; // Objeto del bien

  // id: number;
  // dependencia: string;
  // proy: number;
  // pro: number;
  // cdp: number;
  // fecha: Date;
  // estado: string;
  // rubro: string;
  // valor: number;
  // saldo: number;
  // tipo: string;
  // detalle?: DetalleCDP[];
}
