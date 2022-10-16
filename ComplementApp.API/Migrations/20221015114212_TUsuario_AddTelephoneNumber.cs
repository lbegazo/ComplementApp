using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplementApp.API.Migrations
{
    public partial class TUsuario_AddTelephoneNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "TUsuario",
                type: "VARCHAR(15)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "TUsuario");
        }
    }
}
