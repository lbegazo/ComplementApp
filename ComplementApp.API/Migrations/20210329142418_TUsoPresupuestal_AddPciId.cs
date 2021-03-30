using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TUsoPresupuestal_AddPciId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TUsoPresupuestal",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TUsoPresupuestal_PciId",
                table: "TUsoPresupuestal",
                column: "PciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TUsoPresupuestal_TPci_PciId",
                table: "TUsoPresupuestal",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TUsoPresupuestal_TPci_PciId",
                table: "TUsoPresupuestal");

            migrationBuilder.DropIndex(
                name: "IX_TUsoPresupuestal_PciId",
                table: "TUsoPresupuestal");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TUsoPresupuestal");
        }
    }
}
