using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class UpdateParametroLiquidacionTercero : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GmfAfc",
                table: "TParametroLiquidacionTercero",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GmfAfc",
                table: "TParametroLiquidacionTercero");
        }
    }
}
