using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class ModContratoTipoPagoTipoIvaTipoCxPTipoDocSop_Creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TipoDocumentoSoporte",
                columns: table => new
                {
                    TipoDocumentoSoporteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoDocumentoSoporte", x => x.TipoDocumentoSoporteId);
                });

            migrationBuilder.CreateTable(
                name: "TTipoCuentaXPagar",
                columns: table => new
                {
                    TipoCuentaXPagarId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTipoCuentaXPagar", x => x.TipoCuentaXPagarId);
                });

            migrationBuilder.CreateTable(
                name: "TTipoDePago",
                columns: table => new
                {
                    TipoDePagoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTipoDePago", x => x.TipoDePagoId);
                });

            migrationBuilder.CreateTable(
                name: "TTipoIva",
                columns: table => new
                {
                    TipoIvaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTipoIva", x => x.TipoIvaId);
                });

            migrationBuilder.CreateTable(
                name: "TTipoModalidadContrato",
                columns: table => new
                {
                    TipoModalidadContratoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Nombre = table.Column<string>(type: "VARCHAR(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTipoModalidadContrato", x => x.TipoModalidadContratoId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TipoDocumentoSoporte");

            migrationBuilder.DropTable(
                name: "TTipoCuentaXPagar");

            migrationBuilder.DropTable(
                name: "TTipoDePago");

            migrationBuilder.DropTable(
                name: "TTipoIva");

            migrationBuilder.DropTable(
                name: "TTipoModalidadContrato");
        }
    }
}
