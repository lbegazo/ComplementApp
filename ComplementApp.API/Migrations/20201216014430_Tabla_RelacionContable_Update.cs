using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class Tabla_RelacionContable_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TRelacionContable_TAtributoContable_AtributoContableId",
                table: "TRelacionContable");

            migrationBuilder.DropForeignKey(
                name: "FK_TRelacionContable_TCuentaContable_CuentaContableId",
                table: "TRelacionContable");

            migrationBuilder.AlterColumn<int>(
                name: "RubroPresupuestalId",
                table: "TRelacionContable",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CuentaContableId",
                table: "TRelacionContable",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AtributoContableId",
                table: "TRelacionContable",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TRelacionContable_TAtributoContable_AtributoContableId",
                table: "TRelacionContable",
                column: "AtributoContableId",
                principalTable: "TAtributoContable",
                principalColumn: "AtributoContableId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRelacionContable_TCuentaContable_CuentaContableId",
                table: "TRelacionContable",
                column: "CuentaContableId",
                principalTable: "TCuentaContable",
                principalColumn: "CuentaContableId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TRelacionContable_TAtributoContable_AtributoContableId",
                table: "TRelacionContable");

            migrationBuilder.DropForeignKey(
                name: "FK_TRelacionContable_TCuentaContable_CuentaContableId",
                table: "TRelacionContable");

            migrationBuilder.AlterColumn<int>(
                name: "RubroPresupuestalId",
                table: "TRelacionContable",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CuentaContableId",
                table: "TRelacionContable",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AtributoContableId",
                table: "TRelacionContable",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TRelacionContable_TAtributoContable_AtributoContableId",
                table: "TRelacionContable",
                column: "AtributoContableId",
                principalTable: "TAtributoContable",
                principalColumn: "AtributoContableId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TRelacionContable_TCuentaContable_CuentaContableId",
                table: "TRelacionContable",
                column: "CuentaContableId",
                principalTable: "TCuentaContable",
                principalColumn: "CuentaContableId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
