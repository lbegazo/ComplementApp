namespace ComplementApp.API.Models
{
    public enum EstadoPlanPago
    {
        PorPagar = 4,
        ConSolicitudPago = 27,
        PorObligar = 5,
        ConLiquidacionDeducciones = 6,
        Obligado = 7,
        Pagado = 8,
        Rechazada = 13,
        ConOrdenPago = 19,
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
        RegistradorContable = 5,
        Contratista = 6,
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
        Fijo = 1,
        Variable = 2
    }

    public enum TipoDeIva
    {
        Fijo = 1,
        Variable = 2
    }

    public enum TipoOperacionEnum
    {
        ANULACION = 1,
        REDUCCION,
        ADICION,
        SOLICITUD_INICIAL,
    }

    public enum TipoLista
    {
        ModalidadContrato = 1,
        TipoPago = 2,
        TipoIva = 3,
        TipoCuentaXPagar = 4,
        TipoDocumentoSoporte = 5,
        TipoAdminPila = 6,
        TipoDocumentoIdentidad = 7,
        TipoContrato = 8,
        Pci = 9,
        Dependencia = 10,
        Usuario = 11,
        FuenteFinanciacion = 12,
        SituacionFondo = 13,
        RecursoPresupuestal = 14,
        MedioPago=15,
    }

    public enum TipoOperacionTransaccion
    {
        Creacion = 1,
        Modificacion = 2
    }

    public enum EstadoSolicitudPago
    {
        Generado = 24,
        Aprobado = 25,
        Rechazado = 26,
        ConLiquidacionDeducciones = 27,
    }

    public enum TipoDocumentoArchivo
    {
        CuentaPorPagar = 1,
        Obligacion = 2,
        OrdenPago = 3,
    }

    public enum TipoArchivoObligacion
    {
        Cabecera = 1,
        Deducciones = 2,
        Item = 3,
        Uso = 4,
        Factura = 5,
    }

    public enum TipoArchivoCuentaPorPagar
    {
        Cabecera = 1,
        Detalle = 2,
    }

    public enum Mes
    {
        Enero = 1,
        Febrero = 2,
        Marzo = 3,
        Abril = 4,
        Mayo = 5,
        Junio = 6,
        Julio = 7,
        Agosto = 8,
        Septiembre = 9,
        Octubre = 10,
        Noviembre = 11,
        Diciembre = 12,
    }

    public enum EstadoModificacion
    {
        Insertado = 1,
        Modificado = 2,
        Eliminado = 3,
    }

    public enum EstadoPlanAdquisicion
    {
        Generado = 28,
        ConCDP = 29,
    }

    public enum EstadoDetalleLiquidacion
    {
        Generado = 30,
        Rechazado = 31,
    }
}