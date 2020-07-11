import { DetalleCDP } from './detalleCDP';

export interface Cdp {
  id: number;
  dependencia: string;
  proy: number;
  pro: number;
  cdp: number;
  fecha: Date;
  estado: string;
  rubro: string;
  valor: number;
  saldo: number;
  tipo: string;
  detalle?: DetalleCDP[];
}
