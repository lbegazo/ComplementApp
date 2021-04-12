using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDependencia_Area_AddPci : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TDetalleCDP",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TDependencia",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TArea",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TDetalleCDP_PciId",
                table: "TDetalleCDP",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TDependencia_PciId",
                table: "TDependencia",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TArea_PciId",
                table: "TArea",
                column: "PciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TArea_TPci_PciId",
                table: "TArea",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TDependencia_TPci_PciId",
                table: "TDependencia",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TDetalleCDP_TPci_PciId",
                table: "TDetalleCDP",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TArea_TPci_PciId",
                table: "TArea");

            migrationBuilder.DropForeignKey(
                name: "FK_TDependencia_TPci_PciId",
                table: "TDependencia");

            migrationBuilder.DropForeignKey(
                name: "FK_TDetalleCDP_TPci_PciId",
                table: "TDetalleCDP");

            migrationBuilder.DropIndex(
                name: "IX_TDetalleCDP_PciId",
                table: "TDetalleCDP");

            migrationBuilder.DropIndex(
                name: "IX_TDependencia_PciId",
                table: "TDependencia");

            migrationBuilder.DropIndex(
                name: "IX_TArea_PciId",
                table: "TArea");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TDetalleCDP");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TDependencia");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TArea");
        }
    }
}
