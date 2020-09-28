export interface Transaccion {
  transaccionId: number;
  codigo: string;
  nombre: string;
  descripcion: string;
  icono: string;
  ruta: string;
  padreTransaccionId: number;
  hijos?: Transaccion[];
}
