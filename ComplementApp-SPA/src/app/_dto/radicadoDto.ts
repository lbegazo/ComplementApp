export interface RadicadoDto {
  consecutivo: number;
  planPagoId: number;
  fechaRadicadoSupervisor: Date;
  fechaRadicadoSupervisorDescripcion: string;
  estado: string;
  crp: string;
  nit: string;
  nombreTercero: string;
  numeroRadicadoProveedor: string;
  numeroRadicadoSupervisor: string;
  valorAPagar: number;
  obligacion: string;
  ordenPago: string;
  fechaOrdenPago?: Date;
  fechaOrdenPagoDescripcion: string;
  TextoComprobanteContable: string;
}
