using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TPlanPago_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TPlanPago_TRubroPresupuestal_RubroPresupuestalId",
                table: "TPlanPago");

            migrationBuilder.AlterColumn<int>(
                name: "RubroPresupuestalId",
                table: "TPlanPago",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TPlanPago_TRubroPresupuestal_RubroPresupuestalId",
                table: "TPlanPago",
                column: "RubroPresupuestalId",
                principalTable: "TRubroPresupuestal",
                principalColumn: "RubroPresupuestalId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TPlanPago_TRubroPresupuestal_RubroPresupuestalId",
                table: "TPlanPago");

            migrationBuilder.AlterColumn<int>(
                name: "RubroPresupuestalId",
                table: "TPlanPago",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TPlanPago_TRubroPresupuestal_RubroPresupuestalId",
                table: "TPlanPago",
                column: "RubroPresupuestalId",
                principalTable: "TRubroPresupuestal",
                principalColumn: "RubroPresupuestalId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
