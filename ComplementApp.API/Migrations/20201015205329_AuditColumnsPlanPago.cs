using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class AuditColumnsPlanPago : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "TPlanPago",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "TPlanPago",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdModificacion",
                table: "TPlanPago",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdRegistro",
                table: "TPlanPago",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "TDetalleLiquidacion",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "TDetalleLiquidacion",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModalidadContrato",
                table: "TDetalleLiquidacion",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdModificacion",
                table: "TDetalleLiquidacion",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdRegistro",
                table: "TDetalleLiquidacion",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "TPlanPago");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "TPlanPago");

            migrationBuilder.DropColumn(
                name: "UsuarioIdModificacion",
                table: "TPlanPago");

            migrationBuilder.DropColumn(
                name: "UsuarioIdRegistro",
                table: "TPlanPago");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropColumn(
                name: "ModalidadContrato",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropColumn(
                name: "UsuarioIdModificacion",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropColumn(
                name: "UsuarioIdRegistro",
                table: "TDetalleLiquidacion");
        }
    }
}
