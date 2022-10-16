using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplementApp.API.Migrations
{
    public partial class TPresupuestoFuturo_DecretoFuturo_ModalidadSeleccion_newTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TDecretoFuturo",
                columns: table => new
                {
                    DecretoFuturoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApropiacionVigente = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ApropiacionDisponible = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    RubroPresupuestalId = table.Column<int>(type: "int", nullable: true),
                    PciId = table.Column<int>(type: "int", nullable: true),
                    FuenteFinanciacionId = table.Column<int>(type: "int", nullable: true),
                    SituacionFondoId = table.Column<int>(type: "int", nullable: true),
                    RecursoPresupuestalId = table.Column<int>(type: "int", nullable: true),
                    SaldoProgramado = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDecretoFuturo", x => x.DecretoFuturoId);
                    table.ForeignKey(
                        name: "FK_TDecretoFuturo_TPci_PciId",
                        column: x => x.PciId,
                        principalTable: "TPci",
                        principalColumn: "PciId");
                    table.ForeignKey(
                        name: "FK_TDecretoFuturo_TRubroPresupuestal_RubroPresupuestalId",
                        column: x => x.RubroPresupuestalId,
                        principalTable: "TRubroPresupuestal",
                        principalColumn: "RubroPresupuestalId");
                });

            migrationBuilder.CreateTable(
                name: "TModalidadSeleccion",
                columns: table => new
                {
                    ModalidadSeleccionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TModalidadSeleccion", x => x.ModalidadSeleccionId);
                });

            migrationBuilder.CreateTable(
                name: "TIndicador",
                columns: table => new
                {
                    DetalleDecretoFuturoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    ValorApropiacionVigente = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoPorProgramar = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    RubroPresupuestalId = table.Column<int>(type: "int", nullable: true),
                    DecretoFuturoId = table.Column<int>(type: "int", nullable: false),
                    PciId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIndicador", x => x.DetalleDecretoFuturoId);
                    table.ForeignKey(
                        name: "FK_TIndicador_TDecretoFuturo_DecretoFuturoId",
                        column: x => x.DecretoFuturoId,
                        principalTable: "TDecretoFuturo",
                        principalColumn: "DecretoFuturoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TIndicador_TPci_PciId",
                        column: x => x.PciId,
                        principalTable: "TPci",
                        principalColumn: "PciId");
                    table.ForeignKey(
                        name: "FK_TIndicador_TRubroPresupuestal_RubroPresupuestalId",
                        column: x => x.RubroPresupuestalId,
                        principalTable: "TRubroPresupuestal",
                        principalColumn: "RubroPresupuestalId");
                });

            migrationBuilder.CreateTable(
                name: "TPresupuestoFuturo",
                columns: table => new
                {
                    PresupuestoFuturoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoUNSPSC = table.Column<string>(type: "VARCHAR(8000)", nullable: true),
                    Descripcion = table.Column<string>(type: "VARCHAR(8000)", nullable: true),
                    MesInicio = table.Column<int>(type: "int", nullable: false),
                    MesOferta = table.Column<int>(type: "int", nullable: false),
                    DuracionContrato = table.Column<int>(type: "int", nullable: false),
                    FechaEstimadaContratacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModalidadSeleccionId = table.Column<int>(type: "int", nullable: false),
                    FuenteFinanciacionId = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorActual = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorVigenciaFutura = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoActual = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SeRequiereVigenciaFutura = table.Column<bool>(type: "bit", nullable: false),
                    EstadoVigenciaFuturaId = table.Column<int>(type: "int", nullable: false),
                    UnidadContratacion = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    Ubicacion = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    UsuarioResponsableId = table.Column<int>(type: "int", nullable: false),
                    SeAplicaLey = table.Column<bool>(type: "bit", nullable: false),
                    EsSuministroBYS = table.Column<bool>(type: "bit", nullable: false),
                    DependenciaId = table.Column<int>(type: "int", nullable: false),
                    EstadoActividadPAAId = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PciId = table.Column<int>(type: "int", nullable: false),
                    UsuarioRegistroId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificacionId = table.Column<int>(type: "int", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPresupuestoFuturo", x => x.PresupuestoFuturoId);
                    table.ForeignKey(
                        name: "FK_TPresupuestoFuturo_TDependencia_DependenciaId",
                        column: x => x.DependenciaId,
                        principalTable: "TDependencia",
                        principalColumn: "DependenciaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TPresupuestoFuturo_TFuenteFinanciacion_FuenteFinanciacionId",
                        column: x => x.FuenteFinanciacionId,
                        principalTable: "TFuenteFinanciacion",
                        principalColumn: "FuenteFinanciacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TPresupuestoFuturo_TModalidadSeleccion_ModalidadSeleccionId",
                        column: x => x.ModalidadSeleccionId,
                        principalTable: "TModalidadSeleccion",
                        principalColumn: "ModalidadSeleccionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TPresupuestoFuturo_TPci_PciId",
                        column: x => x.PciId,
                        principalTable: "TPci",
                        principalColumn: "PciId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TPresupuestoFuturo_TUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TUsuario",
                        principalColumn: "UsuarioId");
                    table.ForeignKey(
                        name: "FK_TPresupuestoFuturo_TUsuario_UsuarioResponsableId",
                        column: x => x.UsuarioResponsableId,
                        principalTable: "TUsuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TDetallePresupuestoFuturo",
                columns: table => new
                {
                    DetallePresupuestoFuturoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PresupuestoFuturoId = table.Column<int>(type: "int", nullable: false),
                    DecretoFuturoId = table.Column<int>(type: "int", nullable: false),
                    ValorDecretoFuturo = table.Column<decimal>(type: "decimal(30,8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDetallePresupuestoFuturo", x => x.DetallePresupuestoFuturoId);
                    table.ForeignKey(
                        name: "FK_TDetallePresupuestoFuturo_TDecretoFuturo_DecretoFuturoId",
                        column: x => x.DecretoFuturoId,
                        principalTable: "TDecretoFuturo",
                        principalColumn: "DecretoFuturoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TDetallePresupuestoFuturo_TPresupuestoFuturo_PresupuestoFuturoId",
                        column: x => x.PresupuestoFuturoId,
                        principalTable: "TPresupuestoFuturo",
                        principalColumn: "PresupuestoFuturoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TDecretoFuturo_PciId",
                table: "TDecretoFuturo",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TDecretoFuturo_RubroPresupuestalId",
                table: "TDecretoFuturo",
                column: "RubroPresupuestalId");

            migrationBuilder.CreateIndex(
                name: "IX_TDetallePresupuestoFuturo_DecretoFuturoId",
                table: "TDetallePresupuestoFuturo",
                column: "DecretoFuturoId");

            migrationBuilder.CreateIndex(
                name: "IX_TDetallePresupuestoFuturo_PresupuestoFuturoId",
                table: "TDetallePresupuestoFuturo",
                column: "PresupuestoFuturoId");

            migrationBuilder.CreateIndex(
                name: "IX_TIndicador_DecretoFuturoId",
                table: "TIndicador",
                column: "DecretoFuturoId");

            migrationBuilder.CreateIndex(
                name: "IX_TIndicador_PciId",
                table: "TIndicador",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TIndicador_RubroPresupuestalId",
                table: "TIndicador",
                column: "RubroPresupuestalId");

            migrationBuilder.CreateIndex(
                name: "IX_TPresupuestoFuturo_DependenciaId",
                table: "TPresupuestoFuturo",
                column: "DependenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_TPresupuestoFuturo_FuenteFinanciacionId",
                table: "TPresupuestoFuturo",
                column: "FuenteFinanciacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TPresupuestoFuturo_ModalidadSeleccionId",
                table: "TPresupuestoFuturo",
                column: "ModalidadSeleccionId");

            migrationBuilder.CreateIndex(
                name: "IX_TPresupuestoFuturo_PciId",
                table: "TPresupuestoFuturo",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TPresupuestoFuturo_UsuarioId",
                table: "TPresupuestoFuturo",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TPresupuestoFuturo_UsuarioResponsableId",
                table: "TPresupuestoFuturo",
                column: "UsuarioResponsableId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TDetallePresupuestoFuturo");

            migrationBuilder.DropTable(
                name: "TIndicador");

            migrationBuilder.DropTable(
                name: "TPresupuestoFuturo");

            migrationBuilder.DropTable(
                name: "TDecretoFuturo");

            migrationBuilder.DropTable(
                name: "TModalidadSeleccion");
        }
    }
}
