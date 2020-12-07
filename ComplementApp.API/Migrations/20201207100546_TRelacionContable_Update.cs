using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TRelacionContable_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TRelacionContable_TTipoGasto_TipoGastoId",
                table: "TRelacionContable");

            migrationBuilder.AlterColumn<int>(
                name: "TipoGastoId",
                table: "TRelacionContable",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TRelacionContable_TTipoGasto_TipoGastoId",
                table: "TRelacionContable",
                column: "TipoGastoId",
                principalTable: "TTipoGasto",
                principalColumn: "TipoGastoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TRelacionContable_TTipoGasto_TipoGastoId",
                table: "TRelacionContable");

            migrationBuilder.AlterColumn<int>(
                name: "TipoGastoId",
                table: "TRelacionContable",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TRelacionContable_TTipoGasto_TipoGastoId",
                table: "TRelacionContable",
                column: "TipoGastoId",
                principalTable: "TTipoGasto",
                principalColumn: "TipoGastoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
