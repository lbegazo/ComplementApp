using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class ArchivoDetalleLiquida_Creacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TArchivoDetalleLiquidacion",
                columns: table => new
                {
                    ArchivoDetalleLiquidacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(nullable: false),
                    CantidadRegistro = table.Column<int>(nullable: false),
                    UsuarioIdRegistro = table.Column<int>(nullable: false),
                    FechaRegistro = table.Column<DateTime>(nullable: true),
                    UsuarioIdModificacion = table.Column<int>(nullable: false),
                    FechaModificacion = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TArchivoDetalleLiquidacion", x => x.ArchivoDetalleLiquidacionId);
                });

            migrationBuilder.CreateTable(
                name: "TDetalleArchivoLiquidacion",
                columns: table => new
                {
                    ArchivoDetalleLiquidacionId = table.Column<int>(nullable: false),
                    DetalleLiquidacionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDetalleArchivoLiquidacion", x => new { x.ArchivoDetalleLiquidacionId, x.DetalleLiquidacionId });
                    table.ForeignKey(
                        name: "FK_TDetalleArchivoLiquidacion_TArchivoDetalleLiquidacion_ArchivoDetalleLiquidacionId",
                        column: x => x.ArchivoDetalleLiquidacionId,
                        principalTable: "TArchivoDetalleLiquidacion",
                        principalColumn: "ArchivoDetalleLiquidacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TDetalleArchivoLiquidacion_TDetalleLiquidacion_DetalleLiquidacionId",
                        column: x => x.DetalleLiquidacionId,
                        principalTable: "TDetalleLiquidacion",
                        principalColumn: "DetalleLiquidacionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TDetalleArchivoLiquidacion_DetalleLiquidacionId",
                table: "TDetalleArchivoLiquidacion",
                column: "DetalleLiquidacionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TDetalleArchivoLiquidacion");

            migrationBuilder.DropTable(
                name: "TArchivoDetalleLiquidacion");
        }
    }
}
