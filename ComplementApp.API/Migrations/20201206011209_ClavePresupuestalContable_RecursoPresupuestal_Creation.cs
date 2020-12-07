using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class ClavePresupuestalContable_RecursoPresupuestal_Creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "TParametroGeneral",
                type: "VARCHAR(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TRecursoPresupuestal",
                columns: table => new
                {
                    RecursoPresupuestalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRecursoPresupuestal", x => x.RecursoPresupuestalId);
                });

            migrationBuilder.CreateTable(
                name: "TClavePresupuestalContable",
                columns: table => new
                {
                    ClavePresupuestalContableId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Crp = table.Column<int>(nullable: false),
                    Pci = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Dependencia = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    RubroPresupuestalId = table.Column<int>(nullable: false),
                    TerceroId = table.Column<int>(nullable: false),
                    SituacionFondoId = table.Column<int>(nullable: false),
                    FuenteFinanciacionId = table.Column<int>(nullable: false),
                    RecursoPresupuestalId = table.Column<int>(nullable: false),
                    UsoPresupuestalId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TClavePresupuestalContable", x => x.ClavePresupuestalContableId);
                    table.ForeignKey(
                        name: "FK_TClavePresupuestalContable_TFuenteFinanciacion_FuenteFinanciacionId",
                        column: x => x.FuenteFinanciacionId,
                        principalTable: "TFuenteFinanciacion",
                        principalColumn: "FuenteFinanciacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TClavePresupuestalContable_TRecursoPresupuestal_RecursoPresupuestalId",
                        column: x => x.RecursoPresupuestalId,
                        principalTable: "TRecursoPresupuestal",
                        principalColumn: "RecursoPresupuestalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TClavePresupuestalContable_TRubroPresupuestal_RubroPresupuestalId",
                        column: x => x.RubroPresupuestalId,
                        principalTable: "TRubroPresupuestal",
                        principalColumn: "RubroPresupuestalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TClavePresupuestalContable_TSituacionFondo_SituacionFondoId",
                        column: x => x.SituacionFondoId,
                        principalTable: "TSituacionFondo",
                        principalColumn: "SituacionFondoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TClavePresupuestalContable_TTercero_TerceroId",
                        column: x => x.TerceroId,
                        principalTable: "TTercero",
                        principalColumn: "TerceroId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TClavePresupuestalContable_TUsoPresupuestal_UsoPresupuestalId",
                        column: x => x.UsoPresupuestalId,
                        principalTable: "TUsoPresupuestal",
                        principalColumn: "UsoPresupuestalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TClavePresupuestalContable_FuenteFinanciacionId",
                table: "TClavePresupuestalContable",
                column: "FuenteFinanciacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TClavePresupuestalContable_RecursoPresupuestalId",
                table: "TClavePresupuestalContable",
                column: "RecursoPresupuestalId");

            migrationBuilder.CreateIndex(
                name: "IX_TClavePresupuestalContable_RubroPresupuestalId",
                table: "TClavePresupuestalContable",
                column: "RubroPresupuestalId");

            migrationBuilder.CreateIndex(
                name: "IX_TClavePresupuestalContable_SituacionFondoId",
                table: "TClavePresupuestalContable",
                column: "SituacionFondoId");

            migrationBuilder.CreateIndex(
                name: "IX_TClavePresupuestalContable_TerceroId",
                table: "TClavePresupuestalContable",
                column: "TerceroId");

            migrationBuilder.CreateIndex(
                name: "IX_TClavePresupuestalContable_UsoPresupuestalId",
                table: "TClavePresupuestalContable",
                column: "UsoPresupuestalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TClavePresupuestalContable");

            migrationBuilder.DropTable(
                name: "TRecursoPresupuestal");

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "TParametroGeneral",
                type: "VARCHAR(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(500)",
                oldNullable: true);
        }
    }
}
