import { ValorSeleccion } from './valorSeleccion';

export interface ClavePresupuestalContableDto {
  clavePresupuestalContableId: number;
  cdpId: number;
  crp: number;
  pci: string;
  dependencia: string;
  rubroPresupuestal: ValorSeleccion;
  tercero: ValorSeleccion;
  situacionFondo: ValorSeleccion;
  fuenteFinanciacion: ValorSeleccion;
  recursoPresupuestal: ValorSeleccion;
  usoPresupuestal: ValorSeleccion;
  relacionContable: ValorSeleccion;
}
