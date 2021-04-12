using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDetalleCDP_UpdateRp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rp",
                table: "TDetalleCDP",
                newName: "Crp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Crp",
                table: "TDetalleCDP",
                newName: "Rp");
        }
    }
}
