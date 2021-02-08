import { Usuario } from './usuario';

export interface Contrato {
  contratoId: number;
  crp: number;
  numeroContrato: number;
  fechaRegistro: Date;
  fechaInicio: Date;
  fechaFinal: Date;
  fechaExpedicionPoliza: Date;
  supervisor1: Usuario;
  supervisor2: Usuario;
}
