using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class FormatoSolicitudPago_Creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TFormatoSolicitudPago",
                columns: table => new
                {
                    FormatoSolicitudPagoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TerceroId = table.Column<int>(nullable: false),
                    PlanPagoId = table.Column<int>(nullable: false),
                    Crp = table.Column<long>(nullable: false),
                    FechaInicio = table.Column<DateTime>(nullable: false),
                    FechaFinal = table.Column<DateTime>(nullable: false),
                    valorFacturado = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    MesId = table.Column<int>(nullable: false),
                    ActividadEconomicaId = table.Column<int>(nullable: false),
                    NumeroPlanilla = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    NumeroFactura = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    Observaciones = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    BaseCotizacion = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    EstadoId = table.Column<int>(nullable: false),
                    UsuarioIdRegistro = table.Column<int>(nullable: true),
                    FechaRegistro = table.Column<DateTime>(nullable: true),
                    UsuarioIdModificacion = table.Column<int>(nullable: true),
                    FechaModificacion = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TFormatoSolicitudPago", x => x.FormatoSolicitudPagoId);
                    table.ForeignKey(
                        name: "FK_TFormatoSolicitudPago_TActividadEconomica_ActividadEconomicaId",
                        column: x => x.ActividadEconomicaId,
                        principalTable: "TActividadEconomica",
                        principalColumn: "ActividadEconomicaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TFormatoSolicitudPago_TPlanPago_PlanPagoId",
                        column: x => x.PlanPagoId,
                        principalTable: "TPlanPago",
                        principalColumn: "PlanPagoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TFormatoSolicitudPago_ActividadEconomicaId",
                table: "TFormatoSolicitudPago",
                column: "ActividadEconomicaId");

            migrationBuilder.CreateIndex(
                name: "IX_TFormatoSolicitudPago_PlanPagoId",
                table: "TFormatoSolicitudPago",
                column: "PlanPagoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TFormatoSolicitudPago");
        }
    }
}
