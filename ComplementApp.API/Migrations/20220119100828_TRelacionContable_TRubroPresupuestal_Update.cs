using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TRelacionContable_TRubroPresupuestal_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PadreContableId",
                table: "TRubroPresupuestal",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Estado",
                table: "TRelacionContable",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PadreContableId",
                table: "TRubroPresupuestal");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "TRelacionContable");
        }
    }
}
