using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations.SqlServerMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_Area",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                name: "TB_CDP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dependencia = table.Column<string>(nullable: true),
                    Proy = table.Column<int>(nullable: false),
                    Pro = table.Column<int>(nullable: false),
                    Cdp = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Estado = table.Column<string>(nullable: true),
                    Rubro = table.Column<string>(nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Tipo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CDP", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_DetalleCDP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Crp = table.Column<int>(nullable: false),
                    IdArchivo = table.Column<int>(nullable: false),
                    Cdp = table.Column<int>(nullable: false),
                    Proy = table.Column<int>(nullable: false),
                    Prod = table.Column<int>(nullable: false),
                    Proyecto = table.Column<string>(nullable: true),
                    ActividadBpin = table.Column<string>(nullable: true),
                    PlanDeCompras = table.Column<string>(nullable: true),
                    Responsable = table.Column<string>(nullable: true),
                    Dependencia = table.Column<string>(nullable: true),
                    Rubro = table.Column<string>(nullable: true),
                    ValorAct = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoAct = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorCDP = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorRP = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorOB = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorOP = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Contrato = table.Column<string>(nullable: true),
                    SaldoTotal = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoDisponible = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Area = table.Column<string>(nullable: true),
                    Paa = table.Column<int>(nullable: false),
                    IdSofi = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_DetalleCDP", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_RubroPresupuestal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificacion = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_RubroPresupuestal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_TipoDetalleModificacion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_TipoDetalleModificacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_TipoOperacion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    DayOfBirth = table.Column<DateTime>(nullable: false),
                    LastActive = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    KnownAs = table.Column<string>(nullable: true),
                    Introduction = table.Column<string>(nullable: true),
                    LookingFor = table.Column<string>(nullable: true),
                    Interests = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true),
                    FechaUltimoAcceso = table.Column<DateTime>(nullable: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    Nombres = table.Column<string>(nullable: true),
                    Apellidos = table.Column<string>(nullable: true),
                    CargoId = table.Column<int>(nullable: false),
                    AreaId = table.Column<int>(nullable: false),
                    EsAdministrador = table.Column<bool>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsMain = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId",
                table: "Photos",
                column: "UserId");

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
                name: "Photos");

            migrationBuilder.DropTable(
                name: "TB_CDP");

            migrationBuilder.DropTable(
                name: "TB_DetalleCDP");

            migrationBuilder.DropTable(
                name: "TB_RubroPresupuestal");

            migrationBuilder.DropTable(
                name: "TB_TipoDetalleModificacion");

            migrationBuilder.DropTable(
                name: "TB_TipoOperacion");

            migrationBuilder.DropTable(
                name: "TB_Usuario");

            migrationBuilder.DropTable(
                name: "Values");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "TB_Area");

            migrationBuilder.DropTable(
                name: "TB_Cargo");
        }
    }
}
