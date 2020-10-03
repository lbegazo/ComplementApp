using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class UpdatePlanPagoTable_UsoPresupuestal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TPlanPago_TUsoPresupuestal_UsoPresupuestalId",
                table: "TPlanPago");

            migrationBuilder.DropIndex(
                name: "IX_TPlanPago_UsoPresupuestalId",
                table: "TPlanPago");

            migrationBuilder.AlterColumn<int>(
                name: "UsoPresupuestalId",
                table: "TPlanPago",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UsoPresupuestalId",
                table: "TPlanPago",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TPlanPago_UsoPresupuestalId",
                table: "TPlanPago",
                column: "UsoPresupuestalId");

            migrationBuilder.AddForeignKey(
                name: "FK_TPlanPago_TUsoPresupuestal_UsoPresupuestalId",
                table: "TPlanPago",
                column: "UsoPresupuestalId",
                principalTable: "TUsoPresupuestal",
                principalColumn: "UsoPresupuestalId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
