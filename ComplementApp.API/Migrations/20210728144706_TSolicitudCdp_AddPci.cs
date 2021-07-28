using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TSolicitudCdp_AddPci : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TSolicitudCDP",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TSolicitudCDP_PciId",
                table: "TSolicitudCDP",
                column: "PciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TSolicitudCDP_TPci_PciId",
                table: "TSolicitudCDP",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TSolicitudCDP_TPci_PciId",
                table: "TSolicitudCDP");

            migrationBuilder.DropIndex(
                name: "IX_TSolicitudCDP_PciId",
                table: "TSolicitudCDP");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TSolicitudCDP");
        }
    }
}
