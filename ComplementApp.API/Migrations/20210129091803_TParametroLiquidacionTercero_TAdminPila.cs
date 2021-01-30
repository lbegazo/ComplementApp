using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TParametroLiquidacionTercero_TAdminPila : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsObraPublica",
                table: "TParametroLiquidacionTercero",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MasDeUnContrato",
                table: "TParametroLiquidacionTercero",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TipoAdminPilaId",
                table: "TParametroLiquidacionTercero",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TTipoAdminPila",
                columns: table => new
                {
                    TipoAdminPilaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTipoAdminPila", x => x.TipoAdminPilaId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TParametroLiquidacionTercero_TipoAdminPilaId",
                table: "TParametroLiquidacionTercero",
                column: "TipoAdminPilaId");

            migrationBuilder.AddForeignKey(
                name: "FK_TParametroLiquidacionTercero_TTipoAdminPila_TipoAdminPilaId",
                table: "TParametroLiquidacionTercero",
                column: "TipoAdminPilaId",
                principalTable: "TTipoAdminPila",
                principalColumn: "TipoAdminPilaId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TParametroLiquidacionTercero_TTipoAdminPila_TipoAdminPilaId",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropTable(
                name: "TTipoAdminPila");

            migrationBuilder.DropIndex(
                name: "IX_TParametroLiquidacionTercero_TipoAdminPilaId",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "EsObraPublica",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "MasDeUnContrato",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "TipoAdminPilaId",
                table: "TParametroLiquidacionTercero");
        }
    }
}
