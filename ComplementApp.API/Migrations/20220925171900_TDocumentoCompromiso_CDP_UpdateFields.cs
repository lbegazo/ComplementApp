using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplementApp.API.Migrations
{
    public partial class TDocumentoCompromiso_CDP_UpdateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Cdp",
                table: "TDocumentoCompromiso",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Cdp",
                table: "TDocumentoCompromiso",
                type: "VARCHAR(50)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
