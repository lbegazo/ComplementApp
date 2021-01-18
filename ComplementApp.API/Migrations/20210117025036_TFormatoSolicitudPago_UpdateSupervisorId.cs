using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TFormatoSolicitudPago_UpdateSupervisorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TPlanPago_TRubroPresupuestal_RubroPresupuestalId",
                table: "TPlanPago");

            migrationBuilder.DropIndex(
                name: "IX_TPlanPago_RubroPresupuestalId",
                table: "TPlanPago");

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "TFormatoSolicitudPago",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "TContrato",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "TFormatoSolicitudPago");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "TContrato");

            migrationBuilder.CreateIndex(
                name: "IX_TPlanPago_RubroPresupuestalId",
                table: "TPlanPago",
                column: "RubroPresupuestalId");

            migrationBuilder.AddForeignKey(
                name: "FK_TPlanPago_TRubroPresupuestal_RubroPresupuestalId",
                table: "TPlanPago",
                column: "RubroPresupuestalId",
                principalTable: "TRubroPresupuestal",
                principalColumn: "RubroPresupuestalId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
