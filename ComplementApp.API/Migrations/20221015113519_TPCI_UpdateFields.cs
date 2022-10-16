using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplementApp.API.Migrations
{
    public partial class TPCI_UpdateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsNivelCentral",
                table: "TPci",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsNivelCentral",
                table: "TPci");
        }
    }
}
