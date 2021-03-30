using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TSolicitudPago_ClavePresupuestal_AddPci : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TFormatoSolicitudPago",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TClavePresupuestalContable",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TFormatoSolicitudPago_PciId",
                table: "TFormatoSolicitudPago",
                column: "PciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TFormatoSolicitudPago_TPci_PciId",
                table: "TFormatoSolicitudPago",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TFormatoSolicitudPago_TPci_PciId",
                table: "TFormatoSolicitudPago");

            migrationBuilder.DropIndex(
                name: "IX_TFormatoSolicitudPago_PciId",
                table: "TFormatoSolicitudPago");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TFormatoSolicitudPago");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TClavePresupuestalContable");
        }
    }
}
