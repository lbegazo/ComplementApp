using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TClavePresupuestalContable_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dependencia",
                table: "TDetalleFormatoSolicitudPago",
                type: "VARCHAR(15)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Dependencia",
                table: "TClavePresupuestalContable",
                type: "VARCHAR(15)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(10)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dependencia",
                table: "TDetalleFormatoSolicitudPago");

            migrationBuilder.AlterColumn<string>(
                name: "Dependencia",
                table: "TClavePresupuestalContable",
                type: "VARCHAR(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(15)");
        }
    }
}
