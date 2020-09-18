using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class CreateParametroLiquidacionDeduccion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsAdministrador",
                table: "TUsuario");

            migrationBuilder.CreateTable(
                name: "TParametroGeneral",
                columns: table => new
                {
                    ParametroGeneralId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Descripcion = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    Valor = table.Column<string>(type: "VARCHAR(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TParametroGeneral", x => x.ParametroGeneralId);
                });

            migrationBuilder.CreateTable(
                name: "TParametroLiquidacionTercero",
                columns: table => new
                {
                    ParametroLiquidacionTerceroId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModalidadContrato = table.Column<int>(nullable: false),
                    TipoPago = table.Column<int>(nullable: false),
                    HonorarioSinIva = table.Column<decimal>(type: "decimal(30,8)", nullable: true),
                    BaseAporteSalud = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    AporteSalud = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    AportePension = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    RiesgoLaboral = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    FondoSolidaridad = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    PensionVoluntaria = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Dependiente = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Afc = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    MedicinaPrepagada = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    InteresVivienda = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    FechaInicioDescuentoInteresVivienda = table.Column<DateTime>(nullable: true),
                    FechaFinalDescuentoInteresVivienda = table.Column<DateTime>(nullable: true),
                    TarifaIva = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    TipoIva = table.Column<int>(nullable: true),
                    TipoCuentaPorPagar = table.Column<int>(nullable: true),
                    TipoDocumentoSoporte = table.Column<int>(nullable: false),
                    Debito = table.Column<string>(nullable: true),
                    Credito = table.Column<string>(nullable: true),
                    NumeroCuenta = table.Column<string>(nullable: true),
                    TipoCuenta = table.Column<string>(nullable: true),
                    ConvenioFontic = table.Column<int>(nullable: true),
                    TerceroId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TParametroLiquidacionTercero", x => x.ParametroLiquidacionTerceroId);
                    table.ForeignKey(
                        name: "FK_TParametroLiquidacionTercero_TTercero_TerceroId",
                        column: x => x.TerceroId,
                        principalTable: "TTercero",
                        principalColumn: "TerceroId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TTipoBaseDeduccion",
                columns: table => new
                {
                    TipoBaseDeduccionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTipoBaseDeduccion", x => x.TipoBaseDeduccionId);
                });

            migrationBuilder.CreateTable(
                name: "TDeduccion",
                columns: table => new
                {
                    DeduccionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    Tarifa = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Gmf = table.Column<bool>(nullable: false),
                    estado = table.Column<bool>(nullable: false),
                    TipoBaseDeduccionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDeduccion", x => x.DeduccionId);
                    table.ForeignKey(
                        name: "FK_TDeduccion_TTipoBaseDeduccion_TipoBaseDeduccionId",
                        column: x => x.TipoBaseDeduccionId,
                        principalTable: "TTipoBaseDeduccion",
                        principalColumn: "TipoBaseDeduccionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TTerceroDeduccion",
                columns: table => new
                {
                    TerceroId = table.Column<int>(nullable: false),
                    DeduccionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTerceroDeduccion", x => new { x.TerceroId, x.DeduccionId });
                    table.ForeignKey(
                        name: "FK_TTerceroDeduccion_TDeduccion_DeduccionId",
                        column: x => x.DeduccionId,
                        principalTable: "TDeduccion",
                        principalColumn: "DeduccionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TTerceroDeduccion_TTercero_TerceroId",
                        column: x => x.TerceroId,
                        principalTable: "TTercero",
                        principalColumn: "TerceroId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TDeduccion_TipoBaseDeduccionId",
                table: "TDeduccion",
                column: "TipoBaseDeduccionId");

            migrationBuilder.CreateIndex(
                name: "IX_TParametroLiquidacionTercero_TerceroId",
                table: "TParametroLiquidacionTercero",
                column: "TerceroId");

            migrationBuilder.CreateIndex(
                name: "IX_TTerceroDeduccion_DeduccionId",
                table: "TTerceroDeduccion",
                column: "DeduccionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TParametroGeneral");

            migrationBuilder.DropTable(
                name: "TParametroLiquidacionTercero");

            migrationBuilder.DropTable(
                name: "TTerceroDeduccion");

            migrationBuilder.DropTable(
                name: "TDeduccion");

            migrationBuilder.DropTable(
                name: "TTipoBaseDeduccion");

            migrationBuilder.AddColumn<bool>(
                name: "EsAdministrador",
                table: "TUsuario",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
