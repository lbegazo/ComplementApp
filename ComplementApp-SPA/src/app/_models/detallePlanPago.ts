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

  identificacionTercero: string;
  nombreTercero: string;
  viaticosDescripcion: string;
  crp: number;
  numeroPago: number;
  valorFacturado: number;
  identificacionRubroPresupuestal: string;
  identificacionUsoPresupuestal: string;
  numeroRadicadoSupervisor: string;
  fechaRadicadoSupervisor: Date;
  numeroFactura: string;
  observaciones: string;
}
