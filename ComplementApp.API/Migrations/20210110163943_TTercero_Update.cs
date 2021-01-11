using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TTercero_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TerceroId",
                table: "TUsuario",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TUsuario_TerceroId",
                table: "TUsuario",
                column: "TerceroId");

            migrationBuilder.AddForeignKey(
                name: "FK_TUsuario_TTercero_TerceroId",
                table: "TUsuario",
                column: "TerceroId",
                principalTable: "TTercero",
                principalColumn: "TerceroId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TUsuario_TTercero_TerceroId",
                table: "TUsuario");

            migrationBuilder.DropIndex(
                name: "IX_TUsuario_TerceroId",
                table: "TUsuario");

            migrationBuilder.DropColumn(
                name: "TerceroId",
                table: "TUsuario");
        }
    }
}
