using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TContrato_AddAuditory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInsercion",
                table: "TContrato",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "TContrato",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdModificacion",
                table: "TContrato",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdRegistro",
                table: "TContrato",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaInsercion",
                table: "TContrato");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "TContrato");

            migrationBuilder.DropColumn(
                name: "UsuarioIdModificacion",
                table: "TContrato");

            migrationBuilder.DropColumn(
                name: "UsuarioIdRegistro",
                table: "TContrato");
        }
    }
}
