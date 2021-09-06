import { ValorSeleccion } from '../_dto/valorSeleccion';
import { ActividadEspecifica } from './actividadEspecifica';
import { Cdp } from './cdp';
import { RubroPresupuestal } from './rubroPresupuestal';
import { Usuario } from './usuario';

export class DetalleCDP {
  detalleCdpId: number;
  planAdquisicionId: number;
  pcpId: number;
  idArchivo: number;
  cdp: number;
  proy: number;
  prod: number;
  proyecto: string;
  actividadBpin: string;
  planDeCompras: string;
  dependencia: string;
  dependenciaDescripcion: string;
  valorAct: number;
  saldoAct: number;
  valorInicial: number;
  valorModificacion: number;
  valorCDP: number;
  saldoCDP: number;
  valorRP: number;
  valorOB: number;
  valorOP: number;
  valorTotal: number;
  aplicaContrato: boolean;
  aplicaContratoDescripcion: boolean;
  saldoTotal: number;
  saldoDisponible: number;
  area: string;
  crp: number;
  valorConvenio: number;
  convenio: number;
  decreto: string;
  valorSolicitud: number;
  estadoModificacion: number;

  clavePresupuestalContableId: number;
  esSeleccionada: boolean;
  identificacionPci: string;

  usuarioId: number;
  dependenciaId: number;
  actividadEspecifica: ActividadEspecifica;
  rubroPresupuestal: RubroPresupuestal;
  cdpDocumento: Cdp;
  responsable: ValorSeleccion;
  fechaEstimadaContratacion: Date;
}
