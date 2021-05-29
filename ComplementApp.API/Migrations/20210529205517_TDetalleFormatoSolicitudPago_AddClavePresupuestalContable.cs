using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDetalleFormatoSolicitudPago_AddClavePresupuestalContable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClavePresupuestalContableId",
                table: "TDetalleFormatoSolicitudPago",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TDetalleFormatoSolicitudPago_ClavePresupuestalContableId",
                table: "TDetalleFormatoSolicitudPago",
                column: "ClavePresupuestalContableId");

            migrationBuilder.AddForeignKey(
                name: "FK_TDetalleFormatoSolicitudPago_TClavePresupuestalContable_ClavePresupuestalContableId",
                table: "TDetalleFormatoSolicitudPago",
                column: "ClavePresupuestalContableId",
                principalTable: "TClavePresupuestalContable",
                principalColumn: "ClavePresupuestalContableId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TDetalleFormatoSolicitudPago_TClavePresupuestalContable_ClavePresupuestalContableId",
                table: "TDetalleFormatoSolicitudPago");

            migrationBuilder.DropIndex(
                name: "IX_TDetalleFormatoSolicitudPago_ClavePresupuestalContableId",
                table: "TDetalleFormatoSolicitudPago");

            migrationBuilder.DropColumn(
                name: "ClavePresupuestalContableId",
                table: "TDetalleFormatoSolicitudPago");
        }
    }
}
