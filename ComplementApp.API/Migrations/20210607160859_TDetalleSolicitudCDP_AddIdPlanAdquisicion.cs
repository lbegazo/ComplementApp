using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDetalleSolicitudCDP_AddIdPlanAdquisicion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlanAdquisicionId",
                table: "TDetalleSolicitudCDP",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TDetalleSolicitudCDP_PlanAdquisicionId",
                table: "TDetalleSolicitudCDP",
                column: "PlanAdquisicionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TDetalleSolicitudCDP_TPlanAdquisicion_PlanAdquisicionId",
                table: "TDetalleSolicitudCDP",
                column: "PlanAdquisicionId",
                principalTable: "TPlanAdquisicion",
                principalColumn: "PlanAdquisicionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TDetalleSolicitudCDP_TPlanAdquisicion_PlanAdquisicionId",
                table: "TDetalleSolicitudCDP");

            migrationBuilder.DropIndex(
                name: "IX_TDetalleSolicitudCDP_PlanAdquisicionId",
                table: "TDetalleSolicitudCDP");

            migrationBuilder.DropColumn(
                name: "PlanAdquisicionId",
                table: "TDetalleSolicitudCDP");
        }
    }
}
