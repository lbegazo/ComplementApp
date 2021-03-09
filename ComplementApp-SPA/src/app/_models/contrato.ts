import { Usuario } from './usuario';

export interface Contrato {
  contratoId: number;
  tipoContratoId: number;
  crp: number;
  numeroContrato: number;
  fechaRegistro?: Date;
  fechaInicio: Date;
  fechaFinal: Date;
  fechaExpedicionPoliza: Date;
  supervisor1Id: number;
  supervisor2Id?: number;
  supervisor1?: Usuario;
  supervisor2?: Usuario;
}
