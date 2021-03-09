using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TTipoContrato_CreateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TContrato_TTipoModalidadContrato_TipoModalidadContratoId",
                table: "TContrato");

            migrationBuilder.DropIndex(
                name: "IX_TContrato_TipoModalidadContratoId",
                table: "TContrato");

            migrationBuilder.RenameColumn(
                name: "TipoModalidadContratoId",
                table: "TContrato",
                newName: "TipoContratoId");

            migrationBuilder.CreateTable(
                name: "TTipoContrato",
                columns: table => new
                {
                    TipoContratoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTipoContrato", x => x.TipoContratoId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TTipoContrato");

            migrationBuilder.RenameColumn(
                name: "TipoContratoId",
                table: "TContrato",
                newName: "TipoModalidadContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_TContrato_TipoModalidadContratoId",
                table: "TContrato",
                column: "TipoModalidadContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TContrato_TTipoModalidadContrato_TipoModalidadContratoId",
                table: "TContrato",
                column: "TipoModalidadContratoId",
                principalTable: "TTipoModalidadContrato",
                principalColumn: "TipoModalidadContratoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
