using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TPci_Creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TCDP",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TPci",
                columns: table => new
                {
                    PciId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificacion = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    PadrePciId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPci", x => x.PciId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TCDP_PciId",
                table: "TCDP",
                column: "PciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCDP_TPci_PciId",
                table: "TCDP",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCDP_TPci_PciId",
                table: "TCDP");

            migrationBuilder.DropTable(
                name: "TPci");

            migrationBuilder.DropIndex(
                name: "IX_TCDP_PciId",
                table: "TCDP");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TCDP");
        }
    }
}
