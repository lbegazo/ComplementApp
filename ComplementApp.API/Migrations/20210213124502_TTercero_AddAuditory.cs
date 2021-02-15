using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TTercero_AddAuditory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "TTercero",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "TTercero",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdModificacion",
                table: "TTercero",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdRegistro",
                table: "TTercero",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "TTercero");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "TTercero");

            migrationBuilder.DropColumn(
                name: "UsuarioIdModificacion",
                table: "TTercero");

            migrationBuilder.DropColumn(
                name: "UsuarioIdRegistro",
                table: "TTercero");
        }
    }
}
