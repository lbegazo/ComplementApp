using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDetalleCDP_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "AplicaContrato",
                table: "TDetalleCDP",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(10)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AplicaContrato",
                table: "TDetalleCDP",
                type: "VARCHAR(10)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
