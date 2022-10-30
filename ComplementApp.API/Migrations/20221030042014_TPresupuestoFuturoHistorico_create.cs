using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplementApp.API.Migrations
{
    public partial class TPresupuestoFuturoHistorico_create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TDetallePresupuestoFuturoHistorico",
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
                    table.PrimaryKey("PK_TDetallePresupuestoFuturoHistorico", x => x.DetallePresupuestoFuturoId);
                });

            migrationBuilder.CreateTable(
                name: "TPresupuestoFuturoHistorico",
                columns: table => new
                {
                    PresupuestoFuturoHistoricoId = table.Column<int>(type: "int", nullable: false)
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
                    PresupuestoFuturoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPresupuestoFuturoHistorico", x => x.PresupuestoFuturoHistoricoId);
                    table.ForeignKey(
                        name: "FK_TPresupuestoFuturoHistorico_TDependencia_DependenciaId",
                        column: x => x.DependenciaId,
                        principalTable: "TDependencia",
                        principalColumn: "DependenciaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TPresupuestoFuturoHistorico_TFuenteFinanciacion_FuenteFinanciacionId",
                        column: x => x.FuenteFinanciacionId,
                        principalTable: "TFuenteFinanciacion",
                        principalColumn: "FuenteFinanciacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TPresupuestoFuturoHistorico_TModalidadSeleccion_ModalidadSeleccionId",
                        column: x => x.ModalidadSeleccionId,
                        principalTable: "TModalidadSeleccion",
                        principalColumn: "ModalidadSeleccionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TPresupuestoFuturoHistorico_TPci_PciId",
                        column: x => x.PciId,
                        principalTable: "TPci",
                        principalColumn: "PciId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TPresupuestoFuturoHistorico_TUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TUsuario",
                        principalColumn: "UsuarioId");
                    table.ForeignKey(
                        name: "FK_TPresupuestoFuturoHistorico_TUsuario_UsuarioResponsableId",
                        column: x => x.UsuarioResponsableId,
                        principalTable: "TUsuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TPresupuestoFuturoHistorico_DependenciaId",
                table: "TPresupuestoFuturoHistorico",
                column: "DependenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_TPresupuestoFuturoHistorico_FuenteFinanciacionId",
                table: "TPresupuestoFuturoHistorico",
                column: "FuenteFinanciacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TPresupuestoFuturoHistorico_ModalidadSeleccionId",
                table: "TPresupuestoFuturoHistorico",
                column: "ModalidadSeleccionId");

            migrationBuilder.CreateIndex(
                name: "IX_TPresupuestoFuturoHistorico_PciId",
                table: "TPresupuestoFuturoHistorico",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TPresupuestoFuturoHistorico_UsuarioId",
                table: "TPresupuestoFuturoHistorico",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TPresupuestoFuturoHistorico_UsuarioResponsableId",
                table: "TPresupuestoFuturoHistorico",
                column: "UsuarioResponsableId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TDetallePresupuestoFuturoHistorico");

            migrationBuilder.DropTable(
                name: "TPresupuestoFuturoHistorico");
        }
    }
}
