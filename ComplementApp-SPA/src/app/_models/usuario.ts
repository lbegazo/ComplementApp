export interface Usuario {
  usuarioId: number;
  username: string;
  nombres: string;
  apellidos: string;
  password: string;
  cargoId: number;
  areaId: number;
  fechaCreacion: Date;
  fechaUltimoAcceso: Date;
  cargoNombre: string;
  areaNombre: string;
  esAdministrador: boolean;
}
