export interface LineaPlanPagoDto {
  id: number;
  planPagoId: number;
  mesId: number;
  mesDescripcion: string;
  valor: number;
  estadoModificacion: number;
  viaticos: boolean;
}
