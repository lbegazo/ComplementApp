using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class SolicitudCDP_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoSolicitudCDP",
                table: "TSolicitudCDP");

            migrationBuilder.AddColumn<int>(
                name: "EstadoSolicitudCDPId",
                table: "TSolicitudCDP",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TSolicitudCDP_EstadoSolicitudCDPId",
                table: "TSolicitudCDP",
                column: "EstadoSolicitudCDPId");

            migrationBuilder.AddForeignKey(
                name: "FK_TSolicitudCDP_TEstado_EstadoSolicitudCDPId",
                table: "TSolicitudCDP",
                column: "EstadoSolicitudCDPId",
                principalTable: "TEstado",
                principalColumn: "EstadoId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TSolicitudCDP_TEstado_EstadoSolicitudCDPId",
                table: "TSolicitudCDP");

            migrationBuilder.DropIndex(
                name: "IX_TSolicitudCDP_EstadoSolicitudCDPId",
                table: "TSolicitudCDP");

            migrationBuilder.DropColumn(
                name: "EstadoSolicitudCDPId",
                table: "TSolicitudCDP");

            migrationBuilder.AddColumn<int>(
                name: "EstadoSolicitudCDP",
                table: "TSolicitudCDP",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
