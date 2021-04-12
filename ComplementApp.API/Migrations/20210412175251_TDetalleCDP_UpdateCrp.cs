using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDetalleCDP_UpdateCrp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Crp",
                table: "TDetalleCDP",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Crp",
                table: "TDetalleCDP",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
