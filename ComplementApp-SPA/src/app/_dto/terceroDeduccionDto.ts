import { ValorSeleccion } from './valorSeleccion';

export interface TerceroDeduccionDto {
  tipoIdentificacion: number;
  identificacionTercero: string;
  codigo: string;
  terceroDeduccionId: number;
  tercero: ValorSeleccion;
  deduccion: ValorSeleccion;
  actividadEconomica: ValorSeleccion;
}
