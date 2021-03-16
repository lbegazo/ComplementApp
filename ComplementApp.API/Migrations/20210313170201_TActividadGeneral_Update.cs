using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TActividadGeneral_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValorApropiacion",
                table: "TActividadGeneral",
                newName: "ApropiacionVigente");

            migrationBuilder.RenameColumn(
                name: "SaldoActual",
                table: "TActividadGeneral",
                newName: "ApropiacionDisponible");

            migrationBuilder.AddColumn<int>(
                name: "RubroPresupuestalId",
                table: "TActividadGeneral",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TActividadGeneral_RubroPresupuestalId",
                table: "TActividadGeneral",
                column: "RubroPresupuestalId");

            migrationBuilder.AddForeignKey(
                name: "FK_TActividadGeneral_TRubroPresupuestal_RubroPresupuestalId",
                table: "TActividadGeneral",
                column: "RubroPresupuestalId",
                principalTable: "TRubroPresupuestal",
                principalColumn: "RubroPresupuestalId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TActividadGeneral_TRubroPresupuestal_RubroPresupuestalId",
                table: "TActividadGeneral");

            migrationBuilder.DropIndex(
                name: "IX_TActividadGeneral_RubroPresupuestalId",
                table: "TActividadGeneral");

            migrationBuilder.DropColumn(
                name: "RubroPresupuestalId",
                table: "TActividadGeneral");

            migrationBuilder.RenameColumn(
                name: "ApropiacionVigente",
                table: "TActividadGeneral",
                newName: "ValorApropiacion");

            migrationBuilder.RenameColumn(
                name: "ApropiacionDisponible",
                table: "TActividadGeneral",
                newName: "SaldoActual");
        }
    }
}
