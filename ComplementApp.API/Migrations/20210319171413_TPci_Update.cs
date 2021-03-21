using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TPci_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PadrePciId",
                table: "TPci");

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TUsuario",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Estado",
                table: "TPci",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TUsuario_PciId",
                table: "TUsuario",
                column: "PciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TUsuario_TPci_PciId",
                table: "TUsuario",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TUsuario_TPci_PciId",
                table: "TUsuario");

            migrationBuilder.DropIndex(
                name: "IX_TUsuario_PciId",
                table: "TUsuario");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TUsuario");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "TPci");

            migrationBuilder.AddColumn<int>(
                name: "PadrePciId",
                table: "TPci",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
