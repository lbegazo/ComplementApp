using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class CreateDetalleLiquidacionYDeducciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TDetalleLiquidacion",
                columns: table => new
                {
                    DetalleLiquidacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroIdentificacion = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    Contrato = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    Viaticos = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Crp = table.Column<long>(nullable: false),
                    CantidadPago = table.Column<int>(nullable: false),
                    NumeroPago = table.Column<int>(nullable: false),
                    ValorContrato = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorAdicionReduccion = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorCancelado = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    TotalACancelar = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoActual = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    RubroPresupuestal = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    UsoPresupuestal = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    NombreSupervisor = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    NumeroRadicado = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    FechaRadicado = table.Column<DateTime>(nullable: false),
                    NumeroFactura = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    TextoComprobanteContable = table.Column<string>(type: "VARCHAR(4000)", nullable: true),
                    Honorario = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    HonorarioUvt = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorIva = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    TotalRetenciones = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    TotalAGirar = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    BaseSalud = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    AporteSalud = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    AportePension = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    RiesgoLaboral = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    FondoSolidaridad = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ImpuestoCovid = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SubTotal1 = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    PensionVoluntaria = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Afc = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SubTotal2 = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    MedicinaPrepagada = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Dependientes = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    InteresesVivienda = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    TotalDeducciones = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SubTotal3 = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    RentaExenta = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    LimiteRentaExenta = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    TotalRentaExenta = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    DiferencialRenta = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    BaseGravableRenta = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    BaseGravableUvt = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    MesPagoAnterior = table.Column<int>(nullable: false),
                    MesPagoActual = table.Column<int>(nullable: false),
                    PlanPagoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDetalleLiquidacion", x => x.DetalleLiquidacionId);
                    table.ForeignKey(
                        name: "FK_TDetalleLiquidacion_TPlanPago_PlanPagoId",
                        column: x => x.PlanPagoId,
                        principalTable: "TPlanPago",
                        principalColumn: "PlanPagoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TLiquidacionDeduccion",
                columns: table => new
                {
                    LiquidacionDeduccionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    DeduccionId = table.Column<int>(nullable: false),
                    Tarifa = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Base = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    DetalleLiquidacionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TLiquidacionDeduccion", x => x.LiquidacionDeduccionId);
                    table.ForeignKey(
                        name: "FK_TLiquidacionDeduccion_TDeduccion_DeduccionId",
                        column: x => x.DeduccionId,
                        principalTable: "TDeduccion",
                        principalColumn: "DeduccionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TLiquidacionDeduccion_TDetalleLiquidacion_DetalleLiquidacionId",
                        column: x => x.DetalleLiquidacionId,
                        principalTable: "TDetalleLiquidacion",
                        principalColumn: "DetalleLiquidacionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TDetalleLiquidacion_PlanPagoId",
                table: "TDetalleLiquidacion",
                column: "PlanPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_TLiquidacionDeduccion_DeduccionId",
                table: "TLiquidacionDeduccion",
                column: "DeduccionId");

            migrationBuilder.CreateIndex(
                name: "IX_TLiquidacionDeduccion_DetalleLiquidacionId",
                table: "TLiquidacionDeduccion",
                column: "DetalleLiquidacionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TLiquidacionDeduccion");

            migrationBuilder.DropTable(
                name: "TDetalleLiquidacion");
        }
    }
}
