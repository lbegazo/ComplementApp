using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TCDP_UpdateTerceroId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCDP_TTercero_TerceroId",
                table: "TCDP");

            migrationBuilder.AlterColumn<int>(
                name: "TerceroId",
                table: "TCDP",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TCDP_TTercero_TerceroId",
                table: "TCDP",
                column: "TerceroId",
                principalTable: "TTercero",
                principalColumn: "TerceroId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCDP_TTercero_TerceroId",
                table: "TCDP");

            migrationBuilder.AlterColumn<int>(
                name: "TerceroId",
                table: "TCDP",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TCDP_TTercero_TerceroId",
                table: "TCDP",
                column: "TerceroId",
                principalTable: "TTercero",
                principalColumn: "TerceroId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
