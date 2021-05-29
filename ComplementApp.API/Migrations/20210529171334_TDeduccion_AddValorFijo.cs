using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDeduccion_AddValorFijo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsValorFijo",
                table: "TDeduccion",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorFijo",
                table: "TDeduccion",
                type: "decimal(30,8)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsValorFijo",
                table: "TDeduccion");

            migrationBuilder.DropColumn(
                name: "ValorFijo",
                table: "TDeduccion");
        }
    }
}
