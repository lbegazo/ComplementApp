using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TParametroSistema_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorHash",
                table: "TParametroSistema");

            migrationBuilder.DropColumn(
                name: "ValorSalt",
                table: "TParametroSistema");

            migrationBuilder.AddColumn<string>(
                name: "Valor",
                table: "TParametroSistema",
                type: "VARCHAR(8000)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Valor",
                table: "TParametroSistema");

            migrationBuilder.AddColumn<byte[]>(
                name: "ValorHash",
                table: "TParametroSistema",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ValorSalt",
                table: "TParametroSistema",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
