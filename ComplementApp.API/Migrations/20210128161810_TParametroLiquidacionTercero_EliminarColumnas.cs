using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TParametroLiquidacionTercero_EliminarColumnas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvenioFontic",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "Credito",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "Debito",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "NumeroCuenta",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "TipoCuenta",
                table: "TParametroLiquidacionTercero");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConvenioFontic",
                table: "TParametroLiquidacionTercero",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Credito",
                table: "TParametroLiquidacionTercero",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Debito",
                table: "TParametroLiquidacionTercero",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroCuenta",
                table: "TParametroLiquidacionTercero",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoCuenta",
                table: "TParametroLiquidacionTercero",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
