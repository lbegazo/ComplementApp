using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDetallesFormatoSolicitudPago_Creacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TDetalleFormatoSolicitudPago",
                columns: table => new
                {
                    DetalleFormatoSolicitudPagoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormatoSolicitudPagoId = table.Column<int>(type: "int", nullable: false),
                    RubroPresupuestalId = table.Column<int>(type: "int", nullable: false),
                    ValorAPagar = table.Column<decimal>(type: "decimal(30,8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDetalleFormatoSolicitudPago", x => x.DetalleFormatoSolicitudPagoId);
                    table.ForeignKey(
                        name: "FK_TDetalleFormatoSolicitudPago_TFormatoSolicitudPago_FormatoSolicitudPagoId",
                        column: x => x.FormatoSolicitudPagoId,
                        principalTable: "TFormatoSolicitudPago",
                        principalColumn: "FormatoSolicitudPagoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TDetalleFormatoSolicitudPago_TRubroPresupuestal_RubroPresupuestalId",
                        column: x => x.RubroPresupuestalId,
                        principalTable: "TRubroPresupuestal",
                        principalColumn: "RubroPresupuestalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TDetalleFormatoSolicitudPago_FormatoSolicitudPagoId",
                table: "TDetalleFormatoSolicitudPago",
                column: "FormatoSolicitudPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_TDetalleFormatoSolicitudPago_RubroPresupuestalId",
                table: "TDetalleFormatoSolicitudPago",
                column: "RubroPresupuestalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TDetalleFormatoSolicitudPago");
        }
    }
}
