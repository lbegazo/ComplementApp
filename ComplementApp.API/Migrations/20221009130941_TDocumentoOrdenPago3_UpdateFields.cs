using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplementApp.API.Migrations
{
    public partial class TDocumentoOrdenPago3_UpdateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "OrdenPago",
                table: "TDocumentoOrdenPago",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrdenPago",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
