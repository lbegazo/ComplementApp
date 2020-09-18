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