using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TFormatoSolicitudPago_UpdateValorFacturado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "valorFacturado",
                table: "TFormatoSolicitudPago",
                newName: "ValorFacturado");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValorFacturado",
                table: "TFormatoSolicitudPago",
                newName: "valorFacturado");
        }
    }
}
