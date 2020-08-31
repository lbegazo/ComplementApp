import { DecimalPipe } from '@angular/common';

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
  estadoPlanPagoId: number;
  diasAlPago: number;
  EstadoPlanPagoId: number;
  EstadoPlanPago: string;
  TipoIdentificacionTercero: string;
  IdentificacionTercero: string;
  RubroPresupuestalId: number;
  IdentificacionRubroPresupuestal: string;
  UsoPresupuestalId: number;
  IdentificacionUsoPresupuestal: string;
  EstadoOrdenPagoId: number;
  EstadoOrdenPago: string;
  TerceroId: number;
  esRadicarFactura: boolean;
}
