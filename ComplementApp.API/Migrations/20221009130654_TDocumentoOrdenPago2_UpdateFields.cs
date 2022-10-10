using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplementApp.API.Migrations
{
    public partial class TDocumentoOrdenPago2_UpdateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SolicitudCdp",
                table: "TDocumentoOrdenPago",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)");

            migrationBuilder.AlterColumn<long>(
                name: "CuentasXPagar",
                table: "TDocumentoOrdenPago",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)");

            migrationBuilder.AlterColumn<long>(
                name: "Compromiso",
                table: "TDocumentoOrdenPago",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)");

            migrationBuilder.AlterColumn<long>(
                name: "Cdp",
                table: "TDocumentoOrdenPago",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SolicitudCdp",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "CuentasXPagar",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Compromiso",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Cdp",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
