using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDetalleLiquidacion_AddSolicitudPago : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormatoSolicitudPagoId",
                table: "TDetalleLiquidacion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TDetalleLiquidacion_FormatoSolicitudPagoId",
                table: "TDetalleLiquidacion",
                column: "FormatoSolicitudPagoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TDetalleLiquidacion_TFormatoSolicitudPago_FormatoSolicitudPagoId",
                table: "TDetalleLiquidacion",
                column: "FormatoSolicitudPagoId",
                principalTable: "TFormatoSolicitudPago",
                principalColumn: "FormatoSolicitudPagoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TDetalleLiquidacion_TFormatoSolicitudPago_FormatoSolicitudPagoId",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropIndex(
                name: "IX_TDetalleLiquidacion_FormatoSolicitudPagoId",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropColumn(
                name: "FormatoSolicitudPagoId",
                table: "TDetalleLiquidacion");
        }
    }
}
