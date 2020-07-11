export interface Usuario {
  id: number;
  username: string;
  nombres: string;
  apellidos: string;
  password: string;
  cargoId: number;
  areaId: number;
  fechaCreacion: Date;
  fechaUltimoAcceso: Date;
  cargoDescripcion: string;
  areaDescripcion: string;
  esAdministrador: number;
}
