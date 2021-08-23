using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TPlanAdquisicion_UpdateValorInicial_ValorOperacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValorInicial",
                table: "TPlanAdquisicion",
                type: "decimal(30,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorModificacion",
                table: "TPlanAdquisicion",
                type: "decimal(30,8)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorInicial",
                table: "TPlanAdquisicion");

            migrationBuilder.DropColumn(
                name: "ValorModificacion",
                table: "TPlanAdquisicion");
        }
    }
}
