using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class UpdatePlanPagoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ValorPagado",
                table: "TPlanPago",
                type: "decimal(30,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(30,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorFacturado",
                table: "TPlanPago",
                type: "decimal(30,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(30,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorAdicion",
                table: "TPlanPago",
                type: "decimal(30,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(30,8)");

            migrationBuilder.AlterColumn<long>(
                name: "OrdenPago",
                table: "TPlanPago",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Obligacion",
                table: "TPlanPago",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRadicadoSupervisor",
                table: "TPlanPago",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRadicadoProveedor",
                table: "TPlanPago",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaOrdenPago",
                table: "TPlanPago",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaFactura",
                table: "TPlanPago",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "EstadoPlanPagoId",
                table: "TPlanPago",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "EstadoOrdenPagoId",
                table: "TPlanPago",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DiasAlPago",
                table: "TPlanPago",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ValorPagado",
                table: "TPlanPago",
                type: "decimal(30,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(30,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorFacturado",
                table: "TPlanPago",
                type: "decimal(30,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(30,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorAdicion",
                table: "TPlanPago",
                type: "decimal(30,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(30,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "OrdenPago",
                table: "TPlanPago",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Obligacion",
                table: "TPlanPago",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRadicadoSupervisor",
                table: "TPlanPago",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRadicadoProveedor",
                table: "TPlanPago",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaOrdenPago",
                table: "TPlanPago",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaFactura",
                table: "TPlanPago",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EstadoPlanPagoId",
                table: "TPlanPago",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EstadoOrdenPagoId",
                table: "TPlanPago",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiasAlPago",
                table: "TPlanPago",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
