using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class AddPlanPagoYUsoPresupuestal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TUsoPresupuestal",
                columns: table => new
                {
                    UsoPresupuestalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificacion = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    MarcaAusteridad = table.Column<bool>(nullable: false),
                    RubroPresupuestalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TUsoPresupuestal", x => x.UsoPresupuestalId);
                    table.ForeignKey(
                        name: "FK_TUsoPresupuestal_TRubroPresupuestal_RubroPresupuestalId",
                        column: x => x.RubroPresupuestalId,
                        principalTable: "TRubroPresupuestal",
                        principalColumn: "RubroPresupuestalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TPlanPago",
                columns: table => new
                {
                    PlanPagoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cdp = table.Column<long>(nullable: false),
                    Crp = table.Column<long>(nullable: false),
                    AnioPago = table.Column<int>(nullable: false),
                    MesPago = table.Column<int>(nullable: false),
                    ValorInicial = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorAdicion = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorAPagar = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorPagado = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Viaticos = table.Column<bool>(nullable: false),
                    NumeroPago = table.Column<int>(nullable: false),
                    NumeroRadicadoProveedor = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    FechaRadicadoProveedor = table.Column<DateTime>(nullable: false),
                    NumeroRadicadoSupervisor = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    FechaRadicadoSupervisor = table.Column<DateTime>(nullable: false),
                    NumeroFactura = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    ValorFacturado = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Observaciones = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    FechaFactura = table.Column<DateTime>(nullable: false),
                    Obligacion = table.Column<long>(nullable: false),
                    OrdenPago = table.Column<long>(nullable: false),
                    FechaOrdenPago = table.Column<DateTime>(nullable: false),
                    DiasAlPago = table.Column<int>(nullable: false),
                    EstadoPlanPagoId = table.Column<int>(nullable: false),
                    EstadoOrdenPagoId = table.Column<int>(nullable: false),
                    TerceroId = table.Column<int>(nullable: false),
                    RubroPresupuestalId = table.Column<int>(nullable: false),
                    UsoPresupuestalId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPlanPago", x => x.PlanPagoId);
                    table.ForeignKey(
                        name: "FK_TPlanPago_TRubroPresupuestal_RubroPresupuestalId",
                        column: x => x.RubroPresupuestalId,
                        principalTable: "TRubroPresupuestal",
                        principalColumn: "RubroPresupuestalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TPlanPago_TTercero_TerceroId",
                        column: x => x.TerceroId,
                        principalTable: "TTercero",
                        principalColumn: "TerceroId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TPlanPago_TUsoPresupuestal_UsoPresupuestalId",
                        column: x => x.UsoPresupuestalId,
                        principalTable: "TUsoPresupuestal",
                        principalColumn: "UsoPresupuestalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TPlanPago_RubroPresupuestalId",
                table: "TPlanPago",
                column: "RubroPresupuestalId");

            migrationBuilder.CreateIndex(
                name: "IX_TPlanPago_TerceroId",
                table: "TPlanPago",
                column: "TerceroId");

            migrationBuilder.CreateIndex(
                name: "IX_TPlanPago_UsoPresupuestalId",
                table: "TPlanPago",
                column: "UsoPresupuestalId");

            migrationBuilder.CreateIndex(
                name: "IX_TUsoPresupuestal_RubroPresupuestalId",
                table: "TUsoPresupuestal",
                column: "RubroPresupuestalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TPlanPago");

            migrationBuilder.DropTable(
                name: "TUsoPresupuestal");
        }
    }
}
