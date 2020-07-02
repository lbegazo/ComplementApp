using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TB_TipoOperacion_Cargo_Area_Usuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_Area",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    Estado = table.Column<bool>(nullable: false),
                    EsAdministrable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Area", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_Cargo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    Estado = table.Column<bool>(nullable: false),
                    EsAdministrable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Cargo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_RubroPresupuestal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identificacion = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_RubroPresupuestal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_TipoOperacion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    Estado = table.Column<bool>(nullable: false),
                    EsAdministrable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_TipoOperacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true),
                    FechaUltimoAcceso = table.Column<DateTime>(nullable: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    Nombres = table.Column<string>(nullable: true),
                    Apellidos = table.Column<string>(nullable: true),
                    CargoId = table.Column<int>(nullable: false),
                    AreaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_Usuario_TB_Area_AreaId",
                        column: x => x.AreaId,
                        principalTable: "TB_Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_Usuario_TB_Cargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "TB_Cargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_Usuario_AreaId",
                table: "TB_Usuario",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Usuario_CargoId",
                table: "TB_Usuario",
                column: "CargoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_RubroPresupuestal");

            migrationBuilder.DropTable(
                name: "TB_TipoOperacion");

            migrationBuilder.DropTable(
                name: "TB_Usuario");

            migrationBuilder.DropTable(
                name: "TB_Area");

            migrationBuilder.DropTable(
                name: "TB_Cargo");
        }
    }
}
