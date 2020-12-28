using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TArchivoDetalleLiquidacion_UpdateTipoArchivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoDocumentoArchivo",
                table: "TArchivoDetalleLiquidacion",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoDocumentoArchivo",
                table: "TArchivoDetalleLiquidacion");
        }
    }
}
