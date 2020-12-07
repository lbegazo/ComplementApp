import { RubroPresupuestal } from '../_models/rubroPresupuestal';
import { Tercero } from '../_models/tercero';
import { ValorSeleccion } from './valorSeleccion';

export interface ClavePresupuestalContable {
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
