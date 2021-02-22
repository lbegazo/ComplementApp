using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TTercero_DropColumnFacturaElectronica : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacturadorElectronico",
                table: "TTercero");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FacturadorElectronico",
                table: "TTercero",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
