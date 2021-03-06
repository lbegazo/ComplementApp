import { Transaccion } from './transaccion';

export interface Perfil {
  perfilId: number;
  codigo: string;
  nombre: string;
  descripcion: string;
  estado: boolean;
  checked: boolean;
  transacciones?: Transaccion[];
}
