using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TNumeracion_Create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TNumeracion",
                columns: table => new
                {
                    NumeracionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Consecutivo = table.Column<int>(type: "int", nullable: false),
                    NumeroConsecutivo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    FormatoSolicitudPagoId = table.Column<int>(type: "int", nullable: true),
                    Utilizado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TNumeracion", x => x.NumeracionId);
                    table.ForeignKey(
                        name: "FK_TNumeracion_TFormatoSolicitudPago_FormatoSolicitudPagoId",
                        column: x => x.FormatoSolicitudPagoId,
                        principalTable: "TFormatoSolicitudPago",
                        principalColumn: "FormatoSolicitudPagoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TNumeracion_FormatoSolicitudPagoId",
                table: "TNumeracion",
                column: "FormatoSolicitudPagoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TNumeracion");
        }
    }
}
