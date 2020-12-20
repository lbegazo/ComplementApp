using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TFormatoSolicitudPago_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValorBaseGravableRenta",
                table: "TFormatoSolicitudPago",
                type: "decimal(30,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorIva",
                table: "TFormatoSolicitudPago",
                type: "decimal(30,8)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorBaseGravableRenta",
                table: "TFormatoSolicitudPago");

            migrationBuilder.DropColumn(
                name: "ValorIva",
                table: "TFormatoSolicitudPago");
        }
    }
}
