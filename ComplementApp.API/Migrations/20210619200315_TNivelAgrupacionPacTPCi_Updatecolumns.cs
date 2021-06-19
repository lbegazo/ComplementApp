using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TNivelAgrupacionPacTPCi_Updatecolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nit",
                table: "TPci",
                type: "VARCHAR(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DependenciaAfectacionPAC",
                table: "TNivelAgrupacionPac",
                type: "VARCHAR(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificacionTesoreria",
                table: "TNivelAgrupacionPac",
                type: "VARCHAR(100)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nit",
                table: "TPci");

            migrationBuilder.DropColumn(
                name: "DependenciaAfectacionPAC",
                table: "TNivelAgrupacionPac");

            migrationBuilder.DropColumn(
                name: "IdentificacionTesoreria",
                table: "TNivelAgrupacionPac");
        }
    }
}
