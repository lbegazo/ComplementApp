using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class SolicitudCDP_Detalle_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aprobado",
                table: "TSolicitudCDP");

            migrationBuilder.AddColumn<int>(
                name: "EstadoSolicitudCDP",
                table: "TSolicitudCDP",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoSolicitudCDP",
                table: "TSolicitudCDP");

            migrationBuilder.AddColumn<bool>(
                name: "Aprobado",
                table: "TSolicitudCDP",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
