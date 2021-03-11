using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TParametroLiquidacionTercero_UpdateTipoCuentaxPagar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoCuentaPorPagar",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.RenameColumn(
                name: "TipoDocumentoSoporte",
                table: "TParametroLiquidacionTercero",
                newName: "TipoDocumentoSoporteId");

            migrationBuilder.AddColumn<int>(
                name: "TipoCuentaXPagarId",
                table: "TParametroLiquidacionTercero",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoCuentaXPagarId",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.RenameColumn(
                name: "TipoDocumentoSoporteId",
                table: "TParametroLiquidacionTercero",
                newName: "TipoDocumentoSoporte");

            migrationBuilder.AddColumn<int>(
                name: "TipoCuentaPorPagar",
                table: "TParametroLiquidacionTercero",
                type: "int",
                nullable: true);
        }
    }
}
