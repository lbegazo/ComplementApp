using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TipoGastoFuenteSituacion_Creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TFuenteFinanciacion",
                columns: table => new
                {
                    FuenteFinanciacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TFuenteFinanciacion", x => x.FuenteFinanciacionId);
                });

            migrationBuilder.CreateTable(
                name: "TSituacionFondo",
                columns: table => new
                {
                    SituacionFondoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TSituacionFondo", x => x.SituacionFondoId);
                });

            migrationBuilder.CreateTable(
                name: "TTipoGasto",
                columns: table => new
                {
                    TipoGastoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTipoGasto", x => x.TipoGastoId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TFuenteFinanciacion");

            migrationBuilder.DropTable(
                name: "TSituacionFondo");

            migrationBuilder.DropTable(
                name: "TTipoGasto");
        }
    }
}
