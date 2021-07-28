import { ValorSeleccion } from '../_dto/valorSeleccion';
import { RubroPresupuestal } from './rubroPresupuestal';

export class ActividadGeneral {
  actividadGeneralId: number;
  apropiacionVigente: number;
  apropiacionDisponible: number;
  rubroPresupuestal: RubroPresupuestal;
  situacionFondo: ValorSeleccion;
  fuenteFinanciacion: ValorSeleccion;
  recursoPresupuestal: ValorSeleccion;
}
