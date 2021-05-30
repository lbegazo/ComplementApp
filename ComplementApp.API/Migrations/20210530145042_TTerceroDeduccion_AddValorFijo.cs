using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TTerceroDeduccion_AddValorFijo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorFijo",
                table: "TDeduccion");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorFijo",
                table: "TTerceroDeduccion",
                type: "decimal(30,8)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorFijo",
                table: "TTerceroDeduccion");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorFijo",
                table: "TDeduccion",
                type: "decimal(30,8)",
                nullable: true);
        }
    }
}
