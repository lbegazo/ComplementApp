using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TCargaObligacionTNivelAgrupacionPac_UpdatePCI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TNivelAgrupacionPac",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TCargaObligacion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TNivelAgrupacionPac_PciId",
                table: "TNivelAgrupacionPac",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TCargaObligacion_PciId",
                table: "TCargaObligacion",
                column: "PciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCargaObligacion_TPci_PciId",
                table: "TCargaObligacion",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TNivelAgrupacionPac_TPci_PciId",
                table: "TNivelAgrupacionPac",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCargaObligacion_TPci_PciId",
                table: "TCargaObligacion");

            migrationBuilder.DropForeignKey(
                name: "FK_TNivelAgrupacionPac_TPci_PciId",
                table: "TNivelAgrupacionPac");

            migrationBuilder.DropIndex(
                name: "IX_TNivelAgrupacionPac_PciId",
                table: "TNivelAgrupacionPac");

            migrationBuilder.DropIndex(
                name: "IX_TCargaObligacion_PciId",
                table: "TCargaObligacion");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TNivelAgrupacionPac");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TCargaObligacion");
        }
    }
}
