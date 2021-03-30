using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDetalleLiquidacion_AddPci : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TDetalleLiquidacion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TDetalleLiquidacion_PciId",
                table: "TDetalleLiquidacion",
                column: "PciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TDetalleLiquidacion_TPci_PciId",
                table: "TDetalleLiquidacion",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TDetalleLiquidacion_TPci_PciId",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropIndex(
                name: "IX_TDetalleLiquidacion_PciId",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TDetalleLiquidacion");
        }
    }
}
