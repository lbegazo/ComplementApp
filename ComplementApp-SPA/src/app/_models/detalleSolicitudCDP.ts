import { RubroPresupuestal } from './rubroPresupuestal';

export class DetalleSolicitudCDP {

    detalleSolicitudCDPId: number;
    solicitudCDPId: number;
    saldoActividad: number;
    valorActividad?: number;
    valorSolicitud: number;
    valorCDP?: number;
    saldoCDP?: number;
    rubroPresupuestal: RubroPresupuestal;

}
