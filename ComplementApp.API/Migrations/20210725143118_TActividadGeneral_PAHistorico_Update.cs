using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TActividadGeneral_PAHistorico_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaldoAct",
                table: "TPlanAdquisicionHistorico");

            migrationBuilder.RenameColumn(
                name: "ValorAct",
                table: "TPlanAdquisicionHistorico",
                newName: "Valor");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "TPlanAdquisicionHistorico",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransaccionId",
                table: "TPlanAdquisicionHistorico",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdRegistro",
                table: "TPlanAdquisicionHistorico",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "TPlanAdquisicion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "TPlanAdquisicion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdModificacion",
                table: "TPlanAdquisicion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdRegistro",
                table: "TPlanAdquisicion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FuenteFinanciacionId",
                table: "TActividadGeneral",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecursoPresupuestalId",
                table: "TActividadGeneral",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SituacionFondoId",
                table: "TActividadGeneral",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "TPlanAdquisicionHistorico");

            migrationBuilder.DropColumn(
                name: "TransaccionId",
                table: "TPlanAdquisicionHistorico");

            migrationBuilder.DropColumn(
                name: "UsuarioIdRegistro",
                table: "TPlanAdquisicionHistorico");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "TPlanAdquisicion");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "TPlanAdquisicion");

            migrationBuilder.DropColumn(
                name: "UsuarioIdModificacion",
                table: "TPlanAdquisicion");

            migrationBuilder.DropColumn(
                name: "UsuarioIdRegistro",
                table: "TPlanAdquisicion");

            migrationBuilder.DropColumn(
                name: "FuenteFinanciacionId",
                table: "TActividadGeneral");

            migrationBuilder.DropColumn(
                name: "RecursoPresupuestalId",
                table: "TActividadGeneral");

            migrationBuilder.DropColumn(
                name: "SituacionFondoId",
                table: "TActividadGeneral");

            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "TPlanAdquisicionHistorico",
                newName: "ValorAct");

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoAct",
                table: "TPlanAdquisicionHistorico",
                type: "decimal(30,8)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
