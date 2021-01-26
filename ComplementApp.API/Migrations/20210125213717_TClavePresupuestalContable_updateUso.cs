using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TClavePresupuestalContable_updateUso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TClavePresupuestalContable_TUsoPresupuestal_UsoPresupuestalId",
                table: "TClavePresupuestalContable");

            migrationBuilder.AlterColumn<int>(
                name: "UsoPresupuestalId",
                table: "TClavePresupuestalContable",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TClavePresupuestalContable_TUsoPresupuestal_UsoPresupuestalId",
                table: "TClavePresupuestalContable",
                column: "UsoPresupuestalId",
                principalTable: "TUsoPresupuestal",
                principalColumn: "UsoPresupuestalId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TClavePresupuestalContable_TUsoPresupuestal_UsoPresupuestalId",
                table: "TClavePresupuestalContable");

            migrationBuilder.AlterColumn<int>(
                name: "UsoPresupuestalId",
                table: "TClavePresupuestalContable",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TClavePresupuestalContable_TUsoPresupuestal_UsoPresupuestalId",
                table: "TClavePresupuestalContable",
                column: "UsoPresupuestalId",
                principalTable: "TUsoPresupuestal",
                principalColumn: "UsoPresupuestalId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
