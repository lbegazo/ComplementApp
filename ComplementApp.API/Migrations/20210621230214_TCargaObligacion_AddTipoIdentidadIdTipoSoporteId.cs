using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TCargaObligacion_AddTipoIdentidadIdTipoSoporteId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoDocSoporteCompromiso",
                table: "TCargaObligacion");

            migrationBuilder.DropColumn(
                name: "TipoIdentificacion",
                table: "TCargaObligacion");

            migrationBuilder.AddColumn<int>(
                name: "TipoDocumentoIdentidadId",
                table: "TCargaObligacion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoDocumentoSoporteId",
                table: "TCargaObligacion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TCargaObligacion_TipoDocumentoIdentidadId",
                table: "TCargaObligacion",
                column: "TipoDocumentoIdentidadId");

            migrationBuilder.CreateIndex(
                name: "IX_TCargaObligacion_TipoDocumentoSoporteId",
                table: "TCargaObligacion",
                column: "TipoDocumentoSoporteId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCargaObligacion_TTipoDocumentoIdentidad_TipoDocumentoIdentidadId",
                table: "TCargaObligacion",
                column: "TipoDocumentoIdentidadId",
                principalTable: "TTipoDocumentoIdentidad",
                principalColumn: "TipoDocumentoIdentidadId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TCargaObligacion_TTipoDocumentoSoporte_TipoDocumentoSoporteId",
                table: "TCargaObligacion",
                column: "TipoDocumentoSoporteId",
                principalTable: "TTipoDocumentoSoporte",
                principalColumn: "TipoDocumentoSoporteId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCargaObligacion_TTipoDocumentoIdentidad_TipoDocumentoIdentidadId",
                table: "TCargaObligacion");

            migrationBuilder.DropForeignKey(
                name: "FK_TCargaObligacion_TTipoDocumentoSoporte_TipoDocumentoSoporteId",
                table: "TCargaObligacion");

            migrationBuilder.DropIndex(
                name: "IX_TCargaObligacion_TipoDocumentoIdentidadId",
                table: "TCargaObligacion");

            migrationBuilder.DropIndex(
                name: "IX_TCargaObligacion_TipoDocumentoSoporteId",
                table: "TCargaObligacion");

            migrationBuilder.DropColumn(
                name: "TipoDocumentoIdentidadId",
                table: "TCargaObligacion");

            migrationBuilder.DropColumn(
                name: "TipoDocumentoSoporteId",
                table: "TCargaObligacion");

            migrationBuilder.AddColumn<string>(
                name: "TipoDocSoporteCompromiso",
                table: "TCargaObligacion",
                type: "VARCHAR(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoIdentificacion",
                table: "TCargaObligacion",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "");
        }
    }
}
