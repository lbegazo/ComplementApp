using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TActividadGeneral",
                columns: table => new
                {
                    ActividadGeneralId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    ValorApropiacion = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoActual = table.Column<decimal>(type: "decimal(30,8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TActividadGeneral", x => x.ActividadGeneralId);
                });

            migrationBuilder.CreateTable(
                name: "TArea",
                columns: table => new
                {
                    AreaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Estado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TArea", x => x.AreaId);
                });

            migrationBuilder.CreateTable(
                name: "TCargo",
                columns: table => new
                {
                    CargoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Estado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCargo", x => x.CargoId);
                });

            migrationBuilder.CreateTable(
                name: "TEstado",
                columns: table => new
                {
                    EstadoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Descripcion = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    TipoDocumento = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEstado", x => x.EstadoId);
                });

            migrationBuilder.CreateTable(
                name: "TPerfil",
                columns: table => new
                {
                    PerfilId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Descripcion = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Estado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPerfil", x => x.PerfilId);
                });

            migrationBuilder.CreateTable(
                name: "TRubroPresupuestal",
                columns: table => new
                {
                    RubroPresupuestalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificacion = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    PadreRubroId = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRubroPresupuestal", x => x.RubroPresupuestalId);
                });

            migrationBuilder.CreateTable(
                name: "TTercero",
                columns: table => new
                {
                    TerceroId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoIdentificacion = table.Column<int>(nullable: false),
                    NumeroIdentificacion = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTercero", x => x.TerceroId);
                });

            migrationBuilder.CreateTable(
                name: "TTipoDetalleCDP",
                columns: table => new
                {
                    TipoDetalleCDPId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTipoDetalleCDP", x => x.TipoDetalleCDPId);
                });

            migrationBuilder.CreateTable(
                name: "TTipoOperacion",
                columns: table => new
                {
                    TipoOperacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    Estado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTipoOperacion", x => x.TipoOperacionId);
                });

            migrationBuilder.CreateTable(
                name: "TTransaccion",
                columns: table => new
                {
                    TransaccionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Descripcion = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Estado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTransaccion", x => x.TransaccionId);
                });

            migrationBuilder.CreateTable(
                name: "TUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    DayOfBirth = table.Column<DateTime>(nullable: false),
                    LastActive = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    KnownAs = table.Column<string>(nullable: true),
                    Introduction = table.Column<string>(nullable: true),
                    LookingFor = table.Column<string>(nullable: true),
                    Interests = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TDependencia",
                columns: table => new
                {
                    DependenciaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    AreaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDependencia", x => x.DependenciaId);
                    table.ForeignKey(
                        name: "FK_TDependencia_TArea_AreaId",
                        column: x => x.AreaId,
                        principalTable: "TArea",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TUsuario",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true),
                    FechaUltimoAcceso = table.Column<DateTime>(nullable: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    Nombres = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Apellidos = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    CargoId = table.Column<int>(nullable: true),
                    AreaId = table.Column<int>(nullable: true),
                    EsAdministrador = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TUsuario", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_TUsuario_TArea_AreaId",
                        column: x => x.AreaId,
                        principalTable: "TArea",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TUsuario_TCargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "TCargo",
                        principalColumn: "CargoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TActividadEspecifica",
                columns: table => new
                {
                    ActividadEspecificaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    ValorApropiacionVigente = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoPorProgramar = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    RubroPresupuestalId = table.Column<int>(nullable: true),
                    ActividadGeneralId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TActividadEspecifica", x => x.ActividadEspecificaId);
                    table.ForeignKey(
                        name: "FK_TActividadEspecifica_TActividadGeneral_ActividadGeneralId",
                        column: x => x.ActividadGeneralId,
                        principalTable: "TActividadGeneral",
                        principalColumn: "ActividadGeneralId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TActividadEspecifica_TRubroPresupuestal_RubroPresupuestalId",
                        column: x => x.RubroPresupuestalId,
                        principalTable: "TRubroPresupuestal",
                        principalColumn: "RubroPresupuestalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TCDP",
                columns: table => new
                {
                    CdpId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Instancia = table.Column<int>(nullable: false),
                    Cdp = table.Column<long>(nullable: false),
                    Crp = table.Column<long>(nullable: false),
                    Obligacion = table.Column<long>(nullable: false),
                    OrdenPago = table.Column<long>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Detalle1 = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    ValorInicial = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Operacion = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoActual = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Detalle2 = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Detalle3 = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Detalle4 = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Detalle5 = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Detalle6 = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Detalle7 = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Detalle8 = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Detalle9 = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    Detalle10 = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    RubroPresupuestalId = table.Column<int>(nullable: false),
                    TerceroId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCDP", x => x.CdpId);
                    table.ForeignKey(
                        name: "FK_TCDP_TRubroPresupuestal_RubroPresupuestalId",
                        column: x => x.RubroPresupuestalId,
                        principalTable: "TRubroPresupuestal",
                        principalColumn: "RubroPresupuestalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TCDP_TTercero_TerceroId",
                        column: x => x.TerceroId,
                        principalTable: "TTercero",
                        principalColumn: "TerceroId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TPerfilTransaccion",
                columns: table => new
                {
                    PerfilId = table.Column<int>(nullable: false),
                    TransaccionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPerfilTransaccion", x => new { x.PerfilId, x.TransaccionId });
                    table.ForeignKey(
                        name: "FK_TPerfilTransaccion_TPerfil_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "TPerfil",
                        principalColumn: "PerfilId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TPerfilTransaccion_TTransaccion_TransaccionId",
                        column: x => x.TransaccionId,
                        principalTable: "TTransaccion",
                        principalColumn: "TransaccionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsMain = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_TUser_UserId",
                        column: x => x.UserId,
                        principalTable: "TUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TDetalleCDP",
                columns: table => new
                {
                    DetalleCdpId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PcpId = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    IdArchivo = table.Column<int>(nullable: false),
                    Cdp = table.Column<long>(nullable: false),
                    Proy = table.Column<int>(nullable: false),
                    Prod = table.Column<int>(nullable: false),
                    PlanDeCompras = table.Column<string>(type: "VARCHAR(500)", nullable: true),
                    ValorAct = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoAct = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorCDP = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorRP = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorOB = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    ValorOP = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    AplicaContrato = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    SaldoTotal = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    SaldoDisponible = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Rp = table.Column<int>(nullable: false),
                    Valor_Convenio = table.Column<decimal>(type: "decimal(30,8)", nullable: false),
                    Convenio = table.Column<int>(nullable: false),
                    ActividadGeneralId = table.Column<int>(nullable: false),
                    ActividadEspecificaId = table.Column<int>(nullable: false),
                    UsuarioId = table.Column<int>(nullable: false),
                    DependenciaId = table.Column<int>(nullable: false),
                    AreaId = table.Column<int>(nullable: false),
                    RubroPresupuestalId = table.Column<int>(nullable: false),
                    DecretoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDetalleCDP", x => x.DetalleCdpId);
                    table.ForeignKey(
                        name: "FK_TDetalleCDP_TUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TUsuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TUsuarioPerfil",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(nullable: false),
                    PerfilId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TUsuarioPerfil", x => new { x.UsuarioId, x.PerfilId });
                    table.ForeignKey(
                        name: "FK_TUsuarioPerfil_TPerfil_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "TPerfil",
                        principalColumn: "PerfilId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TUsuarioPerfil_TUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TUsuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId",
                table: "Photos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TActividadEspecifica_ActividadGeneralId",
                table: "TActividadEspecifica",
                column: "ActividadGeneralId");

            migrationBuilder.CreateIndex(
                name: "IX_TActividadEspecifica_RubroPresupuestalId",
                table: "TActividadEspecifica",
                column: "RubroPresupuestalId");

            migrationBuilder.CreateIndex(
                name: "IX_TCDP_RubroPresupuestalId",
                table: "TCDP",
                column: "RubroPresupuestalId");

            migrationBuilder.CreateIndex(
                name: "IX_TCDP_TerceroId",
                table: "TCDP",
                column: "TerceroId");

            migrationBuilder.CreateIndex(
                name: "IX_TDependencia_AreaId",
                table: "TDependencia",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_TDetalleCDP_UsuarioId",
                table: "TDetalleCDP",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TPerfilTransaccion_TransaccionId",
                table: "TPerfilTransaccion",
                column: "TransaccionId");

            migrationBuilder.CreateIndex(
                name: "IX_TUsuario_AreaId",
                table: "TUsuario",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_TUsuario_CargoId",
                table: "TUsuario",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_TUsuarioPerfil_PerfilId",
                table: "TUsuarioPerfil",
                column: "PerfilId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "TActividadEspecifica");

            migrationBuilder.DropTable(
                name: "TCDP");

            migrationBuilder.DropTable(
                name: "TDependencia");

            migrationBuilder.DropTable(
                name: "TDetalleCDP");

            migrationBuilder.DropTable(
                name: "TEstado");

            migrationBuilder.DropTable(
                name: "TPerfilTransaccion");

            migrationBuilder.DropTable(
                name: "TTipoDetalleCDP");

            migrationBuilder.DropTable(
                name: "TTipoOperacion");

            migrationBuilder.DropTable(
                name: "TUsuarioPerfil");

            migrationBuilder.DropTable(
                name: "Values");

            migrationBuilder.DropTable(
                name: "TUser");

            migrationBuilder.DropTable(
                name: "TActividadGeneral");

            migrationBuilder.DropTable(
                name: "TRubroPresupuestal");

            migrationBuilder.DropTable(
                name: "TTercero");

            migrationBuilder.DropTable(
                name: "TTransaccion");

            migrationBuilder.DropTable(
                name: "TPerfil");

            migrationBuilder.DropTable(
                name: "TUsuario");

            migrationBuilder.DropTable(
                name: "TArea");

            migrationBuilder.DropTable(
                name: "TCargo");
        }
    }
}
