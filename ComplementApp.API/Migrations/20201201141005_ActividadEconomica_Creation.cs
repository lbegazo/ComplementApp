using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class ActividadEconomica_Creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActividadEconomicaId",
                table: "TTerceroDeduccion",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TActividadEconomica",
                columns: table => new
                {
                    ActividadEconomicaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(4)", nullable: true),
                    Nombre = table.Column<string>(type: "VARCHAR(1000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TActividadEconomica", x => x.ActividadEconomicaId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TTerceroDeduccion_ActividadEconomicaId",
                table: "TTerceroDeduccion",
                column: "ActividadEconomicaId");

            migrationBuilder.AddForeignKey(
                name: "FK_TTerceroDeduccion_TActividadEconomica_ActividadEconomicaId",
                table: "TTerceroDeduccion",
                column: "ActividadEconomicaId",
                principalTable: "TActividadEconomica",
                principalColumn: "ActividadEconomicaId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TTerceroDeduccion_TActividadEconomica_ActividadEconomicaId",
                table: "TTerceroDeduccion");

            migrationBuilder.DropTable(
                name: "TActividadEconomica");

            migrationBuilder.DropIndex(
                name: "IX_TTerceroDeduccion_ActividadEconomicaId",
                table: "TTerceroDeduccion");

            migrationBuilder.DropColumn(
                name: "ActividadEconomicaId",
                table: "TTerceroDeduccion");
        }
    }
}
