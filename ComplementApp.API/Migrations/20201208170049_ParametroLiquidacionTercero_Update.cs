using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class ParametroLiquidacionTercero_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "TParametroLiquidacionTercero",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "TParametroLiquidacionTercero",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdModificacion",
                table: "TParametroLiquidacionTercero",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdRegistro",
                table: "TParametroLiquidacionTercero",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "UsuarioIdModificacion",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "UsuarioIdRegistro",
                table: "TParametroLiquidacionTercero");
        }
    }
}
