using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TerceroDeduccion_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TTerceroDeduccion",
                table: "TTerceroDeduccion");

            migrationBuilder.AddColumn<int>(
                name: "TerceroDeduccionId",
                table: "TTerceroDeduccion",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TTerceroDeduccion",
                table: "TTerceroDeduccion",
                column: "TerceroDeduccionId");

            migrationBuilder.CreateIndex(
                name: "IX_TTerceroDeduccion_TerceroId",
                table: "TTerceroDeduccion",
                column: "TerceroId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TTerceroDeduccion",
                table: "TTerceroDeduccion");

            migrationBuilder.DropIndex(
                name: "IX_TTerceroDeduccion_TerceroId",
                table: "TTerceroDeduccion");

            migrationBuilder.DropColumn(
                name: "TerceroDeduccionId",
                table: "TTerceroDeduccion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TTerceroDeduccion",
                table: "TTerceroDeduccion",
                columns: new[] { "TerceroId", "DeduccionId" });
        }
    }
}
