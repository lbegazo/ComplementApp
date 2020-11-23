import { Estado } from './estado';
import { TipoOperacion } from './tipoOperacion';
import { Usuario } from './usuario';

export interface SolicitudCDPDto {
  solicitudCDPId: number;
  tipoOperacion: TipoOperacion;
  objetoBienServicioContratado: string;
  usuarioId: number;
  usuario: Usuario;
  fechaRegistro: Date;
  estadoSolicitudCDPId: number;
  estadoSolicitudCDP: Estado;
}
