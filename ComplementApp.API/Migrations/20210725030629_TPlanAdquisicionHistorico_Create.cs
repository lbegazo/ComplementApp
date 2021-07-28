using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TPlanAdquisicionHistorico_Create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TPlanAdquisicionHistorico",
                columns: table => new
                {
                    PlanAdquisicionHistoricoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanAdquisicioId = table.Column<int>(type: "int", nullable: false),
                    PlanDeCompras = table.Column<string>(type: "VARCHAR(500)", nullable: true),
                    ValorAct = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoAct = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    AplicaContrato = table.Column<bool>(type: "bit", nullable: false),
                    Crp = table.Column<long>(type: "bigint", nullable: false),
                    ActividadGeneralId = table.Column<int>(type: "int", nullable: false),
                    ActividadEspecificaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    DependenciaId = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    RubroPresupuestalId = table.Column<int>(type: "int", nullable: false),
                    DecretoId = table.Column<int>(type: "int", nullable: false),
                    PciId = table.Column<int>(type: "int", nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPlanAdquisicionHistorico", x => x.PlanAdquisicionHistoricoId);
                    table.ForeignKey(
                        name: "FK_TPlanAdquisicionHistorico_TPci_PciId",
                        column: x => x.PciId,
                        principalTable: "TPci",
                        principalColumn: "PciId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TPlanAdquisicionHistorico_TUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TUsuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TPlanAdquisicionHistorico_PciId",
                table: "TPlanAdquisicionHistorico",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TPlanAdquisicionHistorico_UsuarioId",
                table: "TPlanAdquisicionHistorico",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TPlanAdquisicionHistorico");
        }
    }
}
