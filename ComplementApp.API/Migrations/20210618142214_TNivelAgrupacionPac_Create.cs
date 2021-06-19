using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TNivelAgrupacionPac_Create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TNivelAgrupacionPac",
                columns: table => new
                {
                    NivelAgrupacionPacId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificacion = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    RubroPresupuestalId = table.Column<int>(type: "int", nullable: false),
                    SituacionFondoId = table.Column<int>(type: "int", nullable: false),
                    FuenteFinanciacionId = table.Column<int>(type: "int", nullable: false),
                    RecursoPresupuestalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TNivelAgrupacionPac", x => x.NivelAgrupacionPacId);
                    table.ForeignKey(
                        name: "FK_TNivelAgrupacionPac_TFuenteFinanciacion_FuenteFinanciacionId",
                        column: x => x.FuenteFinanciacionId,
                        principalTable: "TFuenteFinanciacion",
                        principalColumn: "FuenteFinanciacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TNivelAgrupacionPac_TRecursoPresupuestal_RecursoPresupuestalId",
                        column: x => x.RecursoPresupuestalId,
                        principalTable: "TRecursoPresupuestal",
                        principalColumn: "RecursoPresupuestalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TNivelAgrupacionPac_TRubroPresupuestal_RubroPresupuestalId",
                        column: x => x.RubroPresupuestalId,
                        principalTable: "TRubroPresupuestal",
                        principalColumn: "RubroPresupuestalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TNivelAgrupacionPac_TSituacionFondo_SituacionFondoId",
                        column: x => x.SituacionFondoId,
                        principalTable: "TSituacionFondo",
                        principalColumn: "SituacionFondoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TNivelAgrupacionPac_FuenteFinanciacionId",
                table: "TNivelAgrupacionPac",
                column: "FuenteFinanciacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TNivelAgrupacionPac_RecursoPresupuestalId",
                table: "TNivelAgrupacionPac",
                column: "RecursoPresupuestalId");

            migrationBuilder.CreateIndex(
                name: "IX_TNivelAgrupacionPac_RubroPresupuestalId",
                table: "TNivelAgrupacionPac",
                column: "RubroPresupuestalId");

            migrationBuilder.CreateIndex(
                name: "IX_TNivelAgrupacionPac_SituacionFondoId",
                table: "TNivelAgrupacionPac",
                column: "SituacionFondoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TNivelAgrupacionPac");
        }
    }
}
