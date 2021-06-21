using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TCargaObligacion_AddRubroPresupuestalId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RubroPresupuestalId",
                table: "TCargaObligacion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TCargaObligacion_RubroPresupuestalId",
                table: "TCargaObligacion",
                column: "RubroPresupuestalId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCargaObligacion_TRubroPresupuestal_RubroPresupuestalId",
                table: "TCargaObligacion",
                column: "RubroPresupuestalId",
                principalTable: "TRubroPresupuestal",
                principalColumn: "RubroPresupuestalId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCargaObligacion_TRubroPresupuestal_RubroPresupuestalId",
                table: "TCargaObligacion");

            migrationBuilder.DropIndex(
                name: "IX_TCargaObligacion_RubroPresupuestalId",
                table: "TCargaObligacion");

            migrationBuilder.DropColumn(
                name: "RubroPresupuestalId",
                table: "TCargaObligacion");
        }
    }
}
