import { ValorSeleccion } from './valorSeleccion';

export interface RelacionContableDto {
  relacionContableId: number;
  tipoOperacion: number;
  usoContable: number;
  cuentaContable: ValorSeleccion;
  atributoContable: ValorSeleccion;
  tipoGasto: ValorSeleccion;
}
