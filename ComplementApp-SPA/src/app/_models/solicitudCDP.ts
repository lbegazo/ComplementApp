import { DetalleSolicitudCDP } from './detalleSolicitudCDP';
import { Estado } from './estado';
import { TipoDetalleCDP } from './tipoDetalleCDP';
import { TipoOperacion } from './tipoOperacion';
import { Usuario } from './usuario';

export class SolicitudCDP {
  solicitudCDPId: number;
  fechaSolicitud: Date;
  estadoCDP: string;
  cdp?: number;
  tipoOperacion: TipoOperacion;

  informacionSolicitante: string;
  cargoDescripcion: string;
  areaDescripcion: string;

  numeroActividad: number;
  aplicaContrato: boolean;
  aplicaContratoDescripcion: string;
  nombreBienServicio: string;
  proyectoInversion: string;
  actividadProyectoInversion: string;
  objetoBienServicioContratado: string;

  tipoDetalleCDP?: TipoDetalleCDP;
  observaciones: string;

  usuarioId: number;
  usuario: Usuario;
  fechaRegistro: Date;

  estadoSolicitudCDP: Estado;

  detalleSolicitudCDPs?: DetalleSolicitudCDP[];
}
