import { ValorSeleccion } from './valorSeleccion';

export interface RelacionContableDto {
  relacionContableId: number;
  cuentaContable: ValorSeleccion;
  atributoContable: ValorSeleccion;
  tipoGasto: ValorSeleccion;
}
