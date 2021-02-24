using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TParametroLiquidacionTercero_DropColumnSupervisor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TParametroLiquidacionTercero_TUsuario_SupervisorId",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropIndex(
                name: "IX_TParametroLiquidacionTercero_SupervisorId",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "TParametroLiquidacionTercero");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "TParametroLiquidacionTercero",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TParametroLiquidacionTercero_SupervisorId",
                table: "TParametroLiquidacionTercero",
                column: "SupervisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TParametroLiquidacionTercero_TUsuario_SupervisorId",
                table: "TParametroLiquidacionTercero",
                column: "SupervisorId",
                principalTable: "TUsuario",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
