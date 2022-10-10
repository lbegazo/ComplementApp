using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplementApp.API.Migrations
{
    public partial class TDocumentoOrdenPago_UpdateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Compromisos",
                table: "TDocumentoOrdenPago");

            migrationBuilder.DropColumn(
                name: "OrdenesPago",
                table: "TDocumentoOrdenPago");

            migrationBuilder.AlterColumn<string>(
                name: "SolicitudCdp",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CuentasXPagar",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cdp",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Compromiso",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrdenPago",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Compromiso",
                table: "TDocumentoOrdenPago");

            migrationBuilder.DropColumn(
                name: "OrdenPago",
                table: "TDocumentoOrdenPago");

            migrationBuilder.AlterColumn<string>(
                name: "SolicitudCdp",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)");

            migrationBuilder.AlterColumn<string>(
                name: "CuentasXPagar",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Cdp",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)");

            migrationBuilder.AddColumn<string>(
                name: "Compromisos",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrdenesPago",
                table: "TDocumentoOrdenPago",
                type: "VARCHAR(100)",
                nullable: true);
        }
    }
}
