export interface Usuario {
  id: number;
  username: string;
  nombres: string;
  apellidos: string;
  idCargo: number;
  idArea: number;
  fechaCreacion: Date;
  fechaUltimoAcceso: Date;
  cargoDescripcion: string;
  areaDescripcion: string;
}
