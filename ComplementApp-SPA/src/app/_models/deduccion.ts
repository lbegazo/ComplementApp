import { Tercero } from './tercero';

export interface Deduccion {
  deduccionId: number;
  codigo: string;
  nombre: string;
  tarifa: number;
  base: number;
  valor: number;
  tercero: Tercero;
  esValorFijo: boolean;
  valorFijo: number;
}
