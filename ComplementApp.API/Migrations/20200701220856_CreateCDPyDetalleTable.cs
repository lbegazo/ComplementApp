using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class CreateCDPyDetalleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_CDP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Dependencia = table.Column<string>(nullable: true),
                    Proy = table.Column<int>(nullable: false),
                    Pro = table.Column<int>(nullable: false),
                    Cdp = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Estado = table.Column<string>(nullable: true),
                    Rubro = table.Column<string>(nullable: true),
                    Valor = table.Column<decimal>(nullable: false),
                    Saldo = table.Column<decimal>(nullable: false),
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
                        .Annotation("Sqlite:Autoincrement", true),
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
                    ValorAct = table.Column<decimal>(nullable: false),
                    SaldoAct = table.Column<decimal>(nullable: false),
                    ValorCDP = table.Column<decimal>(nullable: false),
                    ValorRP = table.Column<decimal>(nullable: false),
                    ValorOB = table.Column<decimal>(nullable: false),
                    ValorOP = table.Column<decimal>(nullable: false),
                    Tipo = table.Column<string>(nullable: true),
                    Contrato = table.Column<string>(nullable: true),
                    SaldoTotal = table.Column<decimal>(nullable: false),
                    SaldoDisponible = table.Column<decimal>(nullable: false),
                    Area = table.Column<string>(nullable: true),
                    Paa = table.Column<int>(nullable: false),
                    IdSofi = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_DetalleCDP", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_CDP");

            migrationBuilder.DropTable(
                name: "TB_DetalleCDP");
        }
    }
}
