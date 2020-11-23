using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class SolicitudCDP_Detalle_Creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TSolicitudCDP",
                columns: table => new
                {
                    SolicitudCDPId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaSolicitud = table.Column<DateTime>(nullable: false),
                    EstadoCDP = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    Cdp = table.Column<long>(nullable: true),
                    NumeroActividad = table.Column<int>(nullable: false),
                    AplicaContrato = table.Column<bool>(nullable: false),
                    NombreBienServicio = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    ProyectoInversion = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    ActividadProyectoInversion = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    ObjetoBienServicioContratado = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    Observaciones = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    TipoDetalleCDPId = table.Column<int>(nullable: true),
                    TipoOperacionId = table.Column<int>(nullable: false),
                    Aprobado = table.Column<bool>(nullable: false, defaultValue: false),
                    UsuarioId = table.Column<int>(nullable: false),
                    UsuarioIdRegistro = table.Column<int>(nullable: false),
                    FechaRegistro = table.Column<DateTime>(nullable: false),
                    UsuarioIdModificacion = table.Column<int>(nullable: false),
                    FechaModificacion = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TSolicitudCDP", x => x.SolicitudCDPId);
                    table.ForeignKey(
                        name: "FK_TSolicitudCDP_TTipoDetalleCDP_TipoDetalleCDPId",
                        column: x => x.TipoDetalleCDPId,
                        principalTable: "TTipoDetalleCDP",
                        principalColumn: "TipoDetalleCDPId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TSolicitudCDP_TTipoOperacion_TipoOperacionId",
                        column: x => x.TipoOperacionId,
                        principalTable: "TTipoOperacion",
                        principalColumn: "TipoOperacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TSolicitudCDP_TUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TUsuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TDetalleSolicitudCDP",
                columns: table => new
                {
                    DetalleSolicitudCDPId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaldoActividad = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorActividad = table.Column<decimal>(type: "decimal(30,8)", nullable: true),
                    ValorSolicitud = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorCDP = table.Column<decimal>(type: "decimal(30,8)", nullable: true),
                    SaldoCDP = table.Column<decimal>(type: "decimal(30,8)", nullable: true),
                    RubroPresupuestalId = table.Column<int>(nullable: false),
                    SolicitudCDPId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDetalleSolicitudCDP", x => x.DetalleSolicitudCDPId);
                    table.ForeignKey(
                        name: "FK_TDetalleSolicitudCDP_TRubroPresupuestal_RubroPresupuestalId",
                        column: x => x.RubroPresupuestalId,
                        principalTable: "TRubroPresupuestal",
                        principalColumn: "RubroPresupuestalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TDetalleSolicitudCDP_TSolicitudCDP_SolicitudCDPId",
                        column: x => x.SolicitudCDPId,
                        principalTable: "TSolicitudCDP",
                        principalColumn: "SolicitudCDPId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TDetalleSolicitudCDP_RubroPresupuestalId",
                table: "TDetalleSolicitudCDP",
                column: "RubroPresupuestalId");

            migrationBuilder.CreateIndex(
                name: "IX_TDetalleSolicitudCDP_SolicitudCDPId",
                table: "TDetalleSolicitudCDP",
                column: "SolicitudCDPId");

            migrationBuilder.CreateIndex(
                name: "IX_TSolicitudCDP_TipoDetalleCDPId",
                table: "TSolicitudCDP",
                column: "TipoDetalleCDPId");

            migrationBuilder.CreateIndex(
                name: "IX_TSolicitudCDP_TipoOperacionId",
                table: "TSolicitudCDP",
                column: "TipoOperacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TSolicitudCDP_UsuarioId",
                table: "TSolicitudCDP",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TDetalleSolicitudCDP");

            migrationBuilder.DropTable(
                name: "TSolicitudCDP");
        }
    }
}
