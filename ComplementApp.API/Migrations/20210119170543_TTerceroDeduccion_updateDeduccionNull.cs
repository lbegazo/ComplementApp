using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TTerceroDeduccion_updateDeduccionNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TTerceroDeduccion_TDeduccion_DeduccionId",
                table: "TTerceroDeduccion");

            migrationBuilder.AlterColumn<int>(
                name: "DeduccionId",
                table: "TTerceroDeduccion",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TTerceroDeduccion_TDeduccion_DeduccionId",
                table: "TTerceroDeduccion",
                column: "DeduccionId",
                principalTable: "TDeduccion",
                principalColumn: "DeduccionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TTerceroDeduccion_TDeduccion_DeduccionId",
                table: "TTerceroDeduccion");

            migrationBuilder.AlterColumn<int>(
                name: "DeduccionId",
                table: "TTerceroDeduccion",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TTerceroDeduccion_TDeduccion_DeduccionId",
                table: "TTerceroDeduccion",
                column: "DeduccionId",
                principalTable: "TDeduccion",
                principalColumn: "DeduccionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
