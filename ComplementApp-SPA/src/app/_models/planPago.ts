export interface PlanPago {
  planPagoId: number;
  cdp: number;
  crp: number;
  anioPago: number;
  mesPago: number;
  mesPagoDescripcion: string;
  valorInicial: number;
  valorAdicion: number;
  valorAPagar: number;
  valorPagado: number;
  viaticos: boolean;
  viaticosDescripcion: string;
  numeroPago: number;
  numeroRadicadoProveedor: string;
  fechaRadicadoProveedor: Date;
  numeroRadicadoSupervisor: string;
  fechaRadicadoSupervisor: Date;
  numeroFactura: string;
  valorFacturado: number;
  observaciones: string;
  fechaFactura: Date;
  obligacion: number;
  fechaObligacion: Date;
  ordenPago: number;
  fechaOrdenPago: Date;
  diasAlPago: number;
  estadoPlanPagoId: number;
  estadoPlanPago: string;
  tipoIdentificacionTercero: string;
  identificacionTercero: string;
  nombreTercero: string;
  rubroPresupuestalId: number;
  identificacionRubroPresupuestal: string;
  usoPresupuestalId: number;
  identificacionUsoPresupuestal: string;
  estadoOrdenPagoId: number;
  estadoOrdenPago: string;
  terceroId: number;
  esRadicarFactura: boolean;
  modalidadContrato: number;
  tipoPago: number;
}
