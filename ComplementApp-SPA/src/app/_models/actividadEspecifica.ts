import { ActividadGeneral } from './actividadGeneral';
import { RubroPresupuestal } from './rubroPresupuestal';

export class ActividadEspecifica {
  actividadEspecificaId: number;
  nombre: string;
  valorApropiacionVigente: number;
  saldoPorProgramar: number;
  rubroPresupuestal: RubroPresupuestal;
  actividadGeneral: ActividadGeneral;
  estadoModificacion: number;
}
