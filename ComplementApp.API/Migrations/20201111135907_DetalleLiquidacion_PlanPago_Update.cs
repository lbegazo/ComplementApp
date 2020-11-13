using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class DetalleLiquidacion_PlanPago_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiasAlPago",
                table: "TPlanPago");

            migrationBuilder.DropColumn(
                name: "FechaOrdenPago",
                table: "TPlanPago");

            migrationBuilder.DropColumn(
                name: "Obligacion",
                table: "TPlanPago");

            migrationBuilder.DropColumn(
                name: "OrdenPago",
                table: "TPlanPago");

            migrationBuilder.AddColumn<int>(
                name: "DiasAlPago",
                table: "TDetalleLiquidacion",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaOrdenPago",
                table: "TDetalleLiquidacion",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Obligacion",
                table: "TDetalleLiquidacion",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OrdenPago",
                table: "TDetalleLiquidacion",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiasAlPago",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropColumn(
                name: "FechaOrdenPago",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropColumn(
                name: "Obligacion",
                table: "TDetalleLiquidacion");

            migrationBuilder.DropColumn(
                name: "OrdenPago",
                table: "TDetalleLiquidacion");

            migrationBuilder.AddColumn<int>(
                name: "DiasAlPago",
                table: "TPlanPago",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaOrdenPago",
                table: "TPlanPago",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Obligacion",
                table: "TPlanPago",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OrdenPago",
                table: "TPlanPago",
                type: "bigint",
                nullable: true);
        }
    }
}
