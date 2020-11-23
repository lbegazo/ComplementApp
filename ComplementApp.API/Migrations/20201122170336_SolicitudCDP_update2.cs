using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class SolicitudCDP_update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProyectoInversion",
                table: "TSolicitudCDP",
                type: "VARCHAR(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(250)");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "TSolicitudCDP",
                type: "VARCHAR(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(250)");

            migrationBuilder.AlterColumn<string>(
                name: "ObjetoBienServicioContratado",
                table: "TSolicitudCDP",
                type: "VARCHAR(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(250)");

            migrationBuilder.AlterColumn<string>(
                name: "NombreBienServicio",
                table: "TSolicitudCDP",
                type: "VARCHAR(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(250)");

            migrationBuilder.AlterColumn<string>(
                name: "EstadoCDP",
                table: "TSolicitudCDP",
                type: "VARCHAR(150)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActividadProyectoInversion",
                table: "TSolicitudCDP",
                type: "VARCHAR(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(250)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProyectoInversion",
                table: "TSolicitudCDP",
                type: "VARCHAR(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(500)");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "TSolicitudCDP",
                type: "VARCHAR(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(500)");

            migrationBuilder.AlterColumn<string>(
                name: "ObjetoBienServicioContratado",
                table: "TSolicitudCDP",
                type: "VARCHAR(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(500)");

            migrationBuilder.AlterColumn<string>(
                name: "NombreBienServicio",
                table: "TSolicitudCDP",
                type: "VARCHAR(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(500)");

            migrationBuilder.AlterColumn<string>(
                name: "EstadoCDP",
                table: "TSolicitudCDP",
                type: "VARCHAR(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(150)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActividadProyectoInversion",
                table: "TSolicitudCDP",
                type: "VARCHAR(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(500)");
        }
    }
}
