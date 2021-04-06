using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TActividadGeneral_Especifica_AddPciId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TActividadGeneral",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TActividadEspecifica",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TActividadGeneral_PciId",
                table: "TActividadGeneral",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TActividadEspecifica_PciId",
                table: "TActividadEspecifica",
                column: "PciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TActividadEspecifica_TPci_PciId",
                table: "TActividadEspecifica",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TActividadGeneral_TPci_PciId",
                table: "TActividadGeneral",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TActividadEspecifica_TPci_PciId",
                table: "TActividadEspecifica");

            migrationBuilder.DropForeignKey(
                name: "FK_TActividadGeneral_TPci_PciId",
                table: "TActividadGeneral");

            migrationBuilder.DropIndex(
                name: "IX_TActividadGeneral_PciId",
                table: "TActividadGeneral");

            migrationBuilder.DropIndex(
                name: "IX_TActividadEspecifica_PciId",
                table: "TActividadEspecifica");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TActividadGeneral");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TActividadEspecifica");
        }
    }
}
