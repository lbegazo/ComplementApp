using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TPlanAdquisicion_AddFechaEstimadaContratacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEstimadaContratacion",
                table: "TPlanAdquisicion",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaEstimadaContratacion",
                table: "TPlanAdquisicion");
        }
    }
}
