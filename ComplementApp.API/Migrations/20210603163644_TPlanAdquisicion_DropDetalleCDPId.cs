using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TPlanAdquisicion_DropDetalleCDPId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DetalleCdpId",
                table: "TPlanAdquisicion",
                newName: "PlanAdquisicionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlanAdquisicionId",
                table: "TPlanAdquisicion",
                newName: "DetalleCdpId");
        }
    }
}
