using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TPlanPago_UpdateSaldoDisponible : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValorPagado",
                table: "TPlanPago",
                newName: "SaldoDisponible");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SaldoDisponible",
                table: "TPlanPago",
                newName: "ValorPagado");
        }
    }
}
