using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class UpdateTransaccionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "TTransaccion",
                type: "VARCHAR(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(250)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "TTransaccion",
                type: "VARCHAR(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(10)");

            migrationBuilder.AddColumn<string>(
                name: "Icono",
                table: "TTransaccion",
                type: "VARCHAR(50)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PadreTransaccionId",
                table: "TTransaccion",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Ruta",
                table: "TTransaccion",
                type: "VARCHAR(50)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icono",
                table: "TTransaccion");

            migrationBuilder.DropColumn(
                name: "PadreTransaccionId",
                table: "TTransaccion");

            migrationBuilder.DropColumn(
                name: "Ruta",
                table: "TTransaccion");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "TTransaccion",
                type: "VARCHAR(250)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(250)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "TTransaccion",
                type: "VARCHAR(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)");
        }
    }
}
