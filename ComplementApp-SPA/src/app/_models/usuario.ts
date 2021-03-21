import { Perfil } from './perfil';

export interface Usuario {
  usuarioId: number;
  username: string;
  fechaCreacion: Date;
  fechaUltimoAcceso: Date;
  nombres: string;
  apellidos: string;
  nombreCompleto: string;
  password: string;
  cargoId: number;
  areaId: number;
  cargoNombre: string;
  areaNombre: string;
  numeroIdentificacion: string;
  perfiles?: Perfil[];
  pciId: number;
}
