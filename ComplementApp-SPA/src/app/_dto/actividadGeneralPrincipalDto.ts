import { ActividadEspecifica } from '../_models/actividadEspecifica';
import { ActividadGeneral } from '../_models/actividadGeneral';

export class ActividadGeneralPrincipalDto {
    listaActividadGeneral: ActividadGeneral[];
    listaActividadEspecifica: ActividadEspecifica[];

}
