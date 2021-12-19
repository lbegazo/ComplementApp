using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TCDP_UpdateTerceroId2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCDP_TTercero_TerceroId",
                table: "TCDP");

            migrationBuilder.DropIndex(
                name: "IX_TCDP_TerceroId",
                table: "TCDP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TCDP_TerceroId",
                table: "TCDP",
                column: "TerceroId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCDP_TTercero_TerceroId",
                table: "TCDP",
                column: "TerceroId",
                principalTable: "TTercero",
                principalColumn: "TerceroId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
