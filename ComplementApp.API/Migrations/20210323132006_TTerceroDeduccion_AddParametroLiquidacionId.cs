using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TTerceroDeduccion_AddParametroLiquidacionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParametroLiquidacionTerceroId",
                table: "TTerceroDeduccion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TTerceroDeduccion_ParametroLiquidacionTerceroId",
                table: "TTerceroDeduccion",
                column: "ParametroLiquidacionTerceroId");

            migrationBuilder.AddForeignKey(
                name: "FK_TTerceroDeduccion_TParametroLiquidacionTercero_ParametroLiquidacionTerceroId",
                table: "TTerceroDeduccion",
                column: "ParametroLiquidacionTerceroId",
                principalTable: "TParametroLiquidacionTercero",
                principalColumn: "ParametroLiquidacionTerceroId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TTerceroDeduccion_TParametroLiquidacionTercero_ParametroLiquidacionTerceroId",
                table: "TTerceroDeduccion");

            migrationBuilder.DropIndex(
                name: "IX_TTerceroDeduccion_ParametroLiquidacionTerceroId",
                table: "TTerceroDeduccion");

            migrationBuilder.DropColumn(
                name: "ParametroLiquidacionTerceroId",
                table: "TTerceroDeduccion");
        }
    }
}
