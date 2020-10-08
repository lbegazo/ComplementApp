using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class UpdateTCDPTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Detalle4",
                table: "TCDP",
                type: "VARCHAR(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(250)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Detalle4",
                table: "TCDP",
                type: "VARCHAR(250)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(4000)",
                oldNullable: true);
        }
    }
}
