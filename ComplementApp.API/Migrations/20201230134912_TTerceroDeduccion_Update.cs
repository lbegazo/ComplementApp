using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TTerceroDeduccion_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TerceroDeDeduccionId",
                table: "TTerceroDeduccion",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TerceroId",
                table: "TDeduccion",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TDeduccion_TerceroId",
                table: "TDeduccion",
                column: "TerceroId");

            migrationBuilder.AddForeignKey(
                name: "FK_TDeduccion_TTercero_TerceroId",
                table: "TDeduccion",
                column: "TerceroId",
                principalTable: "TTercero",
                principalColumn: "TerceroId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TDeduccion_TTercero_TerceroId",
                table: "TDeduccion");

            migrationBuilder.DropIndex(
                name: "IX_TDeduccion_TerceroId",
                table: "TDeduccion");

            migrationBuilder.DropColumn(
                name: "TerceroDeDeduccionId",
                table: "TTerceroDeduccion");

            migrationBuilder.DropColumn(
                name: "TerceroId",
                table: "TDeduccion");
        }
    }
}
