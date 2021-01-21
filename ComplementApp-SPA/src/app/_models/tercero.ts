export interface Tercero {
  terceroId: number;
  tipoDocumentoIdentidadId: number;
  tipoDocumentoIdentidad: string;
  numeroIdentificacion: string;
  nombre: string;
  direccion: string;
  email: string;
  telefono: string;
  declaranteRentaDescripcion: string;
  facturadorElectronicoDescripcion: string;
  regimenTributario: string;
  fechaExpedicionDocumento: Date;
  modalidadContrato: number;
}
