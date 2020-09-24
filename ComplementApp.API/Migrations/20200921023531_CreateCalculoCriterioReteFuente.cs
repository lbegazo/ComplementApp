using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class CreateCalculoCriterioReteFuente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TCriterioCalculoReteFuente",
                columns: table => new
                {
                    CriterioCalculoReteFuenteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tarifa = table.Column<decimal>(nullable: false),
                    Desde = table.Column<int>(nullable: false),
                    Hasta = table.Column<int>(nullable: false),
                    Factor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCriterioCalculoReteFuente", x => x.CriterioCalculoReteFuenteId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TCriterioCalculoReteFuente");
        }
    }
}
