using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class RelacionContable_AtributoContable_Creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelacionContableId",
                table: "TClavePresupuestalContable",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TAtributoContable",
                columns: table => new
                {
                    AtributoContableId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAtributoContable", x => x.AtributoContableId);
                });

            migrationBuilder.CreateTable(
                name: "TCuentaContable",
                columns: table => new
                {
                    CuentaContableId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroCuenta = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    DescripcionCuenta = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCuentaContable", x => x.CuentaContableId);
                });

            migrationBuilder.CreateTable(
                name: "TRelacionContable",
                columns: table => new
                {
                    RelacionContableId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuentaContableId = table.Column<int>(nullable: false),
                    AtributoContableId = table.Column<int>(nullable: false),
                    TipoGastoId = table.Column<int>(nullable: false),
                    TipoOperacion = table.Column<int>(nullable: false),
                    UsoContable = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRelacionContable", x => x.RelacionContableId);
                    table.ForeignKey(
                        name: "FK_TRelacionContable_TAtributoContable_AtributoContableId",
                        column: x => x.AtributoContableId,
                        principalTable: "TAtributoContable",
                        principalColumn: "AtributoContableId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TRelacionContable_TCuentaContable_CuentaContableId",
                        column: x => x.CuentaContableId,
                        principalTable: "TCuentaContable",
                        principalColumn: "CuentaContableId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TRelacionContable_TTipoGasto_TipoGastoId",
                        column: x => x.TipoGastoId,
                        principalTable: "TTipoGasto",
                        principalColumn: "TipoGastoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TClavePresupuestalContable_RelacionContableId",
                table: "TClavePresupuestalContable",
                column: "RelacionContableId");

            migrationBuilder.CreateIndex(
                name: "IX_TRelacionContable_AtributoContableId",
                table: "TRelacionContable",
                column: "AtributoContableId");

            migrationBuilder.CreateIndex(
                name: "IX_TRelacionContable_CuentaContableId",
                table: "TRelacionContable",
                column: "CuentaContableId");

            migrationBuilder.CreateIndex(
                name: "IX_TRelacionContable_TipoGastoId",
                table: "TRelacionContable",
                column: "TipoGastoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TClavePresupuestalContable_TRelacionContable_RelacionContableId",
                table: "TClavePresupuestalContable",
                column: "RelacionContableId",
                principalTable: "TRelacionContable",
                principalColumn: "RelacionContableId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TClavePresupuestalContable_TRelacionContable_RelacionContableId",
                table: "TClavePresupuestalContable");

            migrationBuilder.DropTable(
                name: "TRelacionContable");

            migrationBuilder.DropTable(
                name: "TAtributoContable");

            migrationBuilder.DropTable(
                name: "TCuentaContable");

            migrationBuilder.DropIndex(
                name: "IX_TClavePresupuestalContable_RelacionContableId",
                table: "TClavePresupuestalContable");

            migrationBuilder.DropColumn(
                name: "RelacionContableId",
                table: "TClavePresupuestalContable");
        }
    }
}
