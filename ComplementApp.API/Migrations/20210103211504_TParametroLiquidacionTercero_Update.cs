using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TParametroLiquidacionTercero_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FacturaElectronica",
                table: "TParametroLiquidacionTercero",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinalOtrosDescuentos",
                table: "TParametroLiquidacionTercero",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicioOtrosDescuentos",
                table: "TParametroLiquidacionTercero",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtrosDescuentos",
                table: "TParametroLiquidacionTercero",
                type: "decimal(30,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "TParametroLiquidacionTercero",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TParametroLiquidacionTercero_TUsuario_SupervisorId",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropIndex(
                name: "IX_TParametroLiquidacionTercero_SupervisorId",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "FacturaElectronica",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "FechaFinalOtrosDescuentos",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "FechaInicioOtrosDescuentos",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "OtrosDescuentos",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "TParametroLiquidacionTercero");
        }
    }
}
