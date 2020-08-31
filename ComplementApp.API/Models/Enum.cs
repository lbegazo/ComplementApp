namespace ComplementApp.API.Models
{
    public enum TipoDocumento
    {
        ApropiacionDecreto = 1,
        ApropiacionDesagregada = 2,
        Cdp = 3,
        Compromiso = 4,
        Obligacion = 5,
        OrdenPago = 6,
        UsoPresupuestal = 7
    }

    public enum EstadoPlanPago
    {
        PorPagar = 4,
        PorObligar = 5,
        ConLiquidacionDeducciones = 6,
        Obligado = 7,
        Pagado = 8,
        Rechazada = 9,

    }

}