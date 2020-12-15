using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class Contrato_creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TContrato",
                columns: table => new
                {
                    ContratoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroContrato = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    TipoModalidadContratoId = table.Column<int>(nullable: false),
                    FechaRegistro = table.Column<DateTime>(nullable: false),
                    FechaInicio = table.Column<DateTime>(nullable: false),
                    FechaFinal = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TContrato", x => x.ContratoId);
                    table.ForeignKey(
                        name: "FK_TContrato_TTipoModalidadContrato_TipoModalidadContratoId",
                        column: x => x.TipoModalidadContratoId,
                        principalTable: "TTipoModalidadContrato",
                        principalColumn: "TipoModalidadContratoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TContrato_TipoModalidadContratoId",
                table: "TContrato",
                column: "TipoModalidadContratoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TContrato");
        }
    }
}
