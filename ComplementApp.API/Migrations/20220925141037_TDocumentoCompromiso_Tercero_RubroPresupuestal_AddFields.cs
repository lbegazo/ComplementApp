using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplementApp.API.Migrations
{
    public partial class TDocumentoCompromiso_Tercero_RubroPresupuestal_AddFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {           

            migrationBuilder.AddColumn<int>(
                name: "RubroPresupuestalId",
                table: "TDocumentoCompromiso",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TerceroId",
                table: "TDocumentoCompromiso",
                type: "int",
                nullable: true);

           

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RubroPresupuestalId",
                table: "TDocumentoCompromiso");

            migrationBuilder.DropColumn(
                name: "TerceroId",
                table: "TDocumentoCompromiso");     
        }
    }
}
