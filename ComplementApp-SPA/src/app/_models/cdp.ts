export interface Cdp {
  id: number;
  cdpId: number;
  cdp: number;
  crp: number;
  ordenPago: number;
  obligacion: number;
  numeroDocumento: number;

  fecha: Date;
  fechaFormato: string;
  detalle1: string; // Estado
  detalle2: string; // Codigo dependencia
  detalle4: string; // Objeto del bien
  objeto: string; // Objeto del bien completo
  detalle5: string; // Supervisor
  detalle6: string; // Numero Contrado
  detalle7: string; // ModalidadContrato
  detalle8: string; // Fuente Financiacion FTE
  detalle9: string; // Situacion Fondo SIT
  detalle10: string; // Recurso Presupuestal REC
  supervisorId: number;
  terceroId: number;

  valorInicial: number;
  operacion: number;
  valorTotal: number;
  saldoActual: number;

  numeroIdentificacionTercero: string;
  nombreTercero: string;
  identificacionRubro: string;
  nombreRubro: string;
  pci: string;
  cantidadMaxima: number;
  valorPagadoFechaActual: number;
  numeroPagoFechaActual: number;

  formatoSolicitudPagoId: number;
  planPagoId: number;
  contratoId?: number;
  esSeleccionada: boolean;
  valorFacturado: number;
}
