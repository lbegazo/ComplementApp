using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TParametroSistema_Create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TParametroSistema",
                columns: table => new
                {
                    ParametroSistemaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    Descripcion = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    ValorHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ValorSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TParametroSistema", x => x.ParametroSistemaId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TParametroSistema");
        }
    }
}
