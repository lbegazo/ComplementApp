using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class Tercero_Contrato_creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeclaranteRenta",
                table: "TTercero",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "TTercero",
                type: "VARCHAR(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TTercero",
                type: "VARCHAR(100)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FacturadorElectronico",
                table: "TTercero",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "TTercero",
                type: "VARCHAR(20)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeclaranteRenta",
                table: "TTercero");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "TTercero");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "TTercero");

            migrationBuilder.DropColumn(
                name: "FacturadorElectronico",
                table: "TTercero");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "TTercero");
        }
    }
}
