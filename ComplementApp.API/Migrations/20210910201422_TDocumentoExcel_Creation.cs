using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDocumentoExcel_Creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TDocumentoCdp",
                columns: table => new
                {
                    DocumentoCdpId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroDocumento = table.Column<long>(type: "bigint", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoCdp = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Estado = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Dependencia = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    DependenciaDescripcion = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    IdentificacionRubroPresupuestal = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    DescripcionRubroPresupuestal = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    FuenteFinanciacion = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    RecursoPresupuestal = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    SituacionFondo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    ValorInicial = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorOperacion = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorActual = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoPorComprometer = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Objeto = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    SolicitudCdp = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    Compromisos = table.Column<string>(type: "VARCHAR(6000)", nullable: true),
                    CuentasPagar = table.Column<string>(type: "VARCHAR(6000)", nullable: true),
                    Obligaciones = table.Column<string>(type: "VARCHAR(6000)", nullable: true),
                    OrdenesPago = table.Column<string>(type: "VARCHAR(6000)", nullable: true),
                    Reintegros = table.Column<string>(type: "VARCHAR(3000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDocumentoCdp", x => x.DocumentoCdpId);
                });

            migrationBuilder.CreateTable(
                name: "TDocumentoCompromiso",
                columns: table => new
                {
                    DocumentoCompromisoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroDocumento = table.Column<long>(type: "bigint", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Dependencia = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    DependenciaDescripcion = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    IdentificacionRubroPresupuestal = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    DescripcionRubroPresupuestal = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    FuenteFinanciacion = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    RecursoPresupuestal = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    SituacionFondo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    ValorInicial = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorOperacion = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorActual = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoPorUtilizar = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    TipoIdentificacion = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    NumeroIdentificacion = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    NombreRazonSocial = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    MedioPago = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    TipoCuenta = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    NumeroCuenta = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    EstadoCuenta = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    EntidadNit = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    EntidadDescripcion = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    SolicitudCdp = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    Cdp = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    Compromisos = table.Column<string>(type: "VARCHAR(6000)", nullable: true),
                    CuentasXPagar = table.Column<string>(type: "VARCHAR(6000)", nullable: true),
                    Obligaciones = table.Column<string>(type: "VARCHAR(6000)", nullable: true),
                    OrdenesPago = table.Column<string>(type: "VARCHAR(6000)", nullable: true),
                    Reintegros = table.Column<string>(type: "VARCHAR(3000)", nullable: true),
                    FechaDocumentoSoporte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoDocumentoSoporte = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    NumeroDocumentoSoporte = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    Observaciones = table.Column<string>(type: "VARCHAR(250)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDocumentoCompromiso", x => x.DocumentoCompromisoId);
                });

            migrationBuilder.CreateTable(
                name: "TDocumentoObligacion",
                columns: table => new
                {
                    DocumentoObligacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroDocumento = table.Column<long>(type: "bigint", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    ValorActual = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorDeduccion = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorObligadoNoOrdenado = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    TipoIdentificacion = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    NumeroIdentificacion = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    NombreRazonSocial = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    MedioPago = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    TipoCuenta = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    NumeroCuenta = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    EstadoCuenta = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    EntidadNit = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    EntidadDescripcion = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Dependencia = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    DependenciaDescripcion = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    IdentificacionRubroPresupuestal = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    DescripcionRubroPresupuestal = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    ValorInicial = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorOperacion = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorActual2 = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoPorUtilizar = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    FuenteFinanciacion = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    SituacionFondo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    RecursoPresupuestal = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Concepto = table.Column<string>(type: "VARCHAR(3000)", nullable: false),
                    SolicitudCdp = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    Cdp = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    Compromisos = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    CuentasXPagar = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    FechaCuentaXPagar = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Obligaciones = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    OrdenesPago = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    Reintegros = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    FechaDocumentoSoporte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoDocumentoSoporte = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    NumeroDocumentoSoporte = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    ObjetoCompromiso = table.Column<string>(type: "VARCHAR(250)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDocumentoObligacion", x => x.DocumentoObligacionId);
                });

            migrationBuilder.CreateTable(
                name: "TDocumentoOrdenPago",
                columns: table => new
                {
                    DocumentoOrdenPagoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroDocumento = table.Column<long>(type: "bigint", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    ValorBruto = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorDeduccion = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorNeto = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    TipoBeneficiario = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    VigenciaPresupuestal = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    TipoIdentificacion = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    NumeroIdentificacion = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    NombreRazonSocial = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    MedioPago = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    TipoCuenta = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    NumeroCuenta = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    EstadoCuenta = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    EntidadNit = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    EntidadDescripcion = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Dependencia = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    DependenciaDescripcion = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    IdentificacionRubroPresupuestal = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    DescripcionRubroPresupuestal = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    FuenteFinanciacion = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    RecursoPresupuestal = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    SituacionFondo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    ValorPesos = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorMoneda = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorReintegradoPesos = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorReintegradoMoneda = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    TesoreriaPagadora = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    IdentificacionPagaduria = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    CuentaPagaduria = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    Endosada = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    TipoIdentificacion1 = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    NumeroIdentificacion1 = table.Column<string>(type: "VARCHAR(20)", nullable: true),
                    NombreRazonSocial1 = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    NumeroCuenta1 = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    ConceptoPago = table.Column<string>(type: "VARCHAR(1000)", nullable: true),
                    SolicitudCdp = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    Cdp = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    Compromisos = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    CuentasXPagar = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    FechaCuentaXPagar = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Obligaciones = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    OrdenesPago = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    Reintegros = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    FechaDocumentoSoporteCompromiso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoDocumentoSoporteCompromiso = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    NumeroDocumentoSoporteCompromiso = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    ObjetoCompromiso = table.Column<string>(type: "VARCHAR(250)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDocumentoOrdenPago", x => x.DocumentoOrdenPagoId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TDocumentoCdp");

            migrationBuilder.DropTable(
                name: "TDocumentoCompromiso");

            migrationBuilder.DropTable(
                name: "TDocumentoObligacion");

            migrationBuilder.DropTable(
                name: "TDocumentoOrdenPago");
        }
    }
}
