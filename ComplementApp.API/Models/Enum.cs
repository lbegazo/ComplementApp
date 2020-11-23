namespace ComplementApp.API.Models
{
    public enum EstadoPlanPago
    {
        PorPagar = 4,
        PorObligar = 5,
        ConLiquidacionDeducciones = 6,
        Obligado = 7,
        Pagado = 8,
        Rechazada = 13,
    }

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

    public enum PerfilUsuario
    {
        SupervisorContractual = 1,
        Administrador = 2,
        CoordinadorFinanciero = 3,
        RegistradorPresupuesto = 4,
        RegistradorContable = 5
    }

    public enum EstadoSolicitudCDP
    {
        Generado = 16,
        Aprobado = 17,
        Rechazado = 18
    }

    public enum TipoBaseDeducciones
    {
        AFC = 1,
        BASEICA,
        OTRAS,
        RENTA,
        VALORIVA,
        VOLUNTARIO,

    }

    public enum ModalidadContrato
    {
        ContratoPrestacionServicio = 1,
        ProveedorConDescuento,
        ProveedorSinDescuento
    }

    public enum TipoPago
    {
        Fijo = 0,
        Variable = 1
    }

    public enum TipoOperacionEnum
    {
        ANULACION = 1,
        REDUCCION,
        ADICION,
        SOLICITUD_INICIAL,
    }

}