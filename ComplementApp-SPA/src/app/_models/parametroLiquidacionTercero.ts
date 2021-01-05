import { TerceroDeduccionDto } from '../_dto/terceroDeduccionDto';

export interface ParametroLiquidacionTercero {
  parametroLiquidacionTerceroId: number;
  modalidadContrato: number;
  tipoPago: number;
  honorarioSinIva: number;
  baseAporteSalud: number;
  aporteSalud: number;
  aportePension: number;
  riesgoLaboral: number;
  fondoSolidaridad: number;
  pensionVoluntaria: number;
  dependiente: number;
  afc: number;
  medicinaPrepagada: number;
  interesVivienda: number;
  fechaInicioDescuentoInteresVivienda: Date;
  fechaFinalDescuentoInteresVivienda: Date;
  tarifaIva: number;
  tipoIva: number;
  tipoCuentaPorPagar: number;
  tipoDocumentoSoporte: number;
  debito: string;
  credito: string;
  numeroCuenta: string;
  tipoCuenta: string;
  convenioFontic: number;
  terceroId: number;
  terceroDeducciones?: TerceroDeduccionDto[];

  facturaElectronicaId: number;
  supervisorId: number;
  otrosDescuentos: number;
  fechaInicioOtrosDescuentos: Date;
  fechaFinalOtrosDescuentos: Date;
}
