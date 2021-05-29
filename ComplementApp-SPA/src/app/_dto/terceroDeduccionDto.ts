import { DeduccionDto } from './deduccionDto';
import { ValorSeleccion } from './valorSeleccion';

export interface TerceroDeduccionDto {
  terceroDeduccionId: number;
  tipoIdentificacion: number;
  identificacionTercero: string;
  codigo: string;
  tercero: ValorSeleccion;
  deduccion: DeduccionDto;
  actividadEconomica: ValorSeleccion;
  terceroDeDeduccion: ValorSeleccion;
  estadoModificacion: number;
  esValorFijo: boolean;
  valorFijo: number;
}
