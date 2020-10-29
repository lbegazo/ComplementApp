using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDetalleLiquidacion_Salud : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MesSaludActual",
                table: "TDetalleLiquidacion",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MesSaludAnterior",
                table: "TDetalleLiquidacion",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TerceroId",
                table: "TDetalleLiquidacion",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MesSaludActual",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropColumn(
                name: "MesSaludAnterior",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropColumn(
                name: "TerceroId",
                table: "TDetalleLiquidacion");
        }
    }
}
