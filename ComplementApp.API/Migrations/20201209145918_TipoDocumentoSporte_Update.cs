using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TipoDocumentoSporte_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TipoDocumentoSoporte",
                table: "TipoDocumentoSoporte");

            migrationBuilder.RenameTable(
                name: "TipoDocumentoSoporte",
                newName: "TTipoDocumentoSoporte");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TTipoDocumentoSoporte",
                table: "TTipoDocumentoSoporte",
                column: "TipoDocumentoSoporteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TTipoDocumentoSoporte",
                table: "TTipoDocumentoSoporte");

            migrationBuilder.RenameTable(
                name: "TTipoDocumentoSoporte",
                newName: "TipoDocumentoSoporte");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipoDocumentoSoporte",
                table: "TipoDocumentoSoporte",
                column: "TipoDocumentoSoporteId");
        }
    }
}
