using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TCargaObligacion_Create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TCargaObligacion",
                columns: table => new
                {
                    CargaObligacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroDocumento = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    ValorActual = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorDeduccion = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorObligadoNoOrdenado = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    TipoIdentificacion = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    NumeroIdentificacion = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    NombreRazonSocial = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    MedioPago = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    TipoCuenta = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    NumeroCuenta = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    EstadoCuenta = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    EntidadNit = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    EntidadDescripcion = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    Dependencia = table.Column<string>(type: "VARCHAR(3)", nullable: false),
                    DependenciaDescripcion = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    RubroIdentificacion = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    RubroDescripcion = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    ValorInicial = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorOperacion = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorActual2 = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoPorUtilizar = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    FuenteFinanciacion = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    SituacionFondo = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    RecursoPresupuestal = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Concepto = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    SolicitudCdp = table.Column<int>(type: "int", nullable: false),
                    Cdp = table.Column<int>(type: "int", nullable: false),
                    Compromiso = table.Column<int>(type: "int", nullable: false),
                    CuentaPorPagar = table.Column<int>(type: "int", nullable: true),
                    FechaCuentaPorPagar = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Obligacion = table.Column<int>(type: "int", nullable: false),
                    OrdenPago = table.Column<long>(type: "bigint", nullable: false),
                    Reintegro = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    FechaDocSoporteCompromiso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoDocSoporteCompromiso = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    NumeroDocSoporteCompromiso = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    ObjetoCompromiso = table.Column<string>(type: "VARCHAR(250)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCargaObligacion", x => x.CargaObligacionId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TCargaObligacion");
        }
    }
}
