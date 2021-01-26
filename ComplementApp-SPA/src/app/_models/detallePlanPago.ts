export interface DetallePlanPago {
  planPagoId: number;
  terceroId: number;
  detalle4: string;
  detalle5: string;
  detalle6: string;
  detalle7: string;
  valorTotal: number;
  saldoActual: number;
  fecha: Date;
  operacion: number;
  viaticos: boolean;

  identificacionTercero: string;
  nombreTercero: string;
  viaticosDescripcion: string;
  crp: number;
  numeroPago: number;
  cantidadPago: number;
  valorFacturado: number;
  identificacionRubroPresupuestal: string;
  identificacionUsoPresupuestal: string;
  numeroRadicadoSupervisor: string;
  numeroRadicadoProveedor: string;
  fechaRadicadoSupervisor: Date;
  numeroFactura: string;
  observaciones: string;
  modalidadContrato: number;
  tipoPago: number;
  fechaRadicadoSupervisorFormato: string;
  fechaRadicadoProveedorFormato: string;
}
