using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TParametroLiquidacionTercero_NotasLegales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MasDeUnContrato",
                table: "TParametroLiquidacionTercero",
                newName: "NotaLegal5");

            migrationBuilder.RenameColumn(
                name: "EsObraPublica",
                table: "TParametroLiquidacionTercero",
                newName: "NotaLegal4");

            migrationBuilder.RenameColumn(
                name: "SupervisorId",
                table: "TContrato",
                newName: "Supervisor2Id");

            migrationBuilder.AddColumn<bool>(
                name: "NotaLegal1",
                table: "TParametroLiquidacionTercero",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotaLegal2",
                table: "TParametroLiquidacionTercero",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotaLegal3",
                table: "TParametroLiquidacionTercero",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaExpedicionPoliza",
                table: "TContrato",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Supervisor1Id",
                table: "TContrato",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotaLegal1",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "NotaLegal2",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "NotaLegal3",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "FechaExpedicionPoliza",
                table: "TContrato");

            migrationBuilder.DropColumn(
                name: "Supervisor1Id",
                table: "TContrato");

            migrationBuilder.RenameColumn(
                name: "NotaLegal5",
                table: "TParametroLiquidacionTercero",
                newName: "MasDeUnContrato");

            migrationBuilder.RenameColumn(
                name: "NotaLegal4",
                table: "TParametroLiquidacionTercero",
                newName: "EsObraPublica");

            migrationBuilder.RenameColumn(
                name: "Supervisor2Id",
                table: "TContrato",
                newName: "SupervisorId");
        }
    }
}
