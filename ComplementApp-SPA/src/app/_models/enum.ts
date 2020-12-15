export class Enum<T> {
  public constructor(public readonly value: T) {}
  public toString() {
    return this.value.toString();
  }
}

export class PrimaryColor extends Enum<string> {
  public static readonly Red = new Enum('#FF0000');
  public static readonly Green = new Enum('#00FF00');
  public static readonly Blue = new Enum('#0000FF');
}

export class Color extends PrimaryColor {
  public static readonly White = new Enum('#FFFFFF');
  public static readonly Black = new Enum('#000000');
}

export class EstadoPlanPago extends Enum<number> {
  public static readonly PorPagar = new Enum(4);
  public static readonly PorObligar = new Enum(5);
  public static readonly ConLiquidacionDeducciones = new Enum(6);
  public static readonly Obligado = new Enum(7);
  public static readonly Pagado = new Enum(8);
  public static readonly Rechazada = new Enum(13);
  public static readonly ConOrdenPago = new Enum(19);
}

export class ModalidadContrato extends Enum<number> {
  public static readonly ContratoPrestacionServicio = new Enum(1);
  public static readonly ProveedorConDescuento = new Enum(2);
  public static readonly ProveedorSinDescuento = new Enum(3);
}

export class TipoPago extends Enum<number> {
  public static readonly Fijo = new Enum(0);
  public static readonly Variable = new Enum(1);
}

export class EstadoSolicitudCDP extends Enum<number> {
  public static readonly Generado = new Enum(16);
  public static readonly Aprobado = new Enum(17);
  public static readonly Rechazado = new Enum(18);
}

export class PerfilUsuario extends Enum<number> {
  public static readonly SupervisorContractual = new Enum(1);
  public static readonly Administrador = new Enum(2);
  public static readonly CoordinadorFinanciero = new Enum(3);
  public static readonly RegistradorPresupuesto = new Enum(4);
  public static readonly RegistradorContable = new Enum(5);
}

export class TipoLista extends Enum<number> {
  public static readonly ModalidadContrato = new Enum(1);
  public static readonly TipoPago = new Enum(2);
  public static readonly TipoIva = new Enum(3);
  public static readonly TipoCuentaXPagar = new Enum(4);
  public static readonly TipoDocumentoSoporte = new Enum(5);
}

export class EstadoSolicitudPago extends Enum<number> {
  public static readonly Generado = new Enum(24);
  public static readonly Aprobado = new Enum(25);
  public static readonly Rechazado = new Enum(26);
}


/*
console.log(PrimaryColor.Red);
// Output: Enum { value: '#FF0000' }
console.log(Color.Red); // inherited!
// Output: Enum { value: '#FF0000' }
console.log(Color.Red.value); // we have to call .value to get the value.
// Output: #FF0000
console.log(Color.Red.toString()); // toString() works too.
// Output: #FF0000

class Thing {
  color: Color;
}

let thing: Thing = {
  color: Color.Red,
};

switch (thing.color) {
  case Color.Red: // ...
  case Color.White: // ...
}
*/