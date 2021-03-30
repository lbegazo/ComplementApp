using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TArchivoDetalleLiquidacion_TNumeracion_AddPciId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TNumeracion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TArchivoDetalleLiquidacion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TNumeracion_PciId",
                table: "TNumeracion",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TArchivoDetalleLiquidacion_PciId",
                table: "TArchivoDetalleLiquidacion",
                column: "PciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TArchivoDetalleLiquidacion_TPci_PciId",
                table: "TArchivoDetalleLiquidacion",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TNumeracion_TPci_PciId",
                table: "TNumeracion",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TArchivoDetalleLiquidacion_TPci_PciId",
                table: "TArchivoDetalleLiquidacion");

            migrationBuilder.DropForeignKey(
                name: "FK_TNumeracion_TPci_PciId",
                table: "TNumeracion");

            migrationBuilder.DropIndex(
                name: "IX_TNumeracion_PciId",
                table: "TNumeracion");

            migrationBuilder.DropIndex(
                name: "IX_TArchivoDetalleLiquidacion_PciId",
                table: "TArchivoDetalleLiquidacion");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TNumeracion");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TArchivoDetalleLiquidacion");
        }
    }
}
