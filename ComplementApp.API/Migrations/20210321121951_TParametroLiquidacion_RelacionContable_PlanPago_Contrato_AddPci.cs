using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TParametroLiquidacion_RelacionContable_PlanPago_Contrato_AddPci : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TRelacionContable",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TPlanPago",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TParametroLiquidacionTercero",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TContrato",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TRelacionContable_PciId",
                table: "TRelacionContable",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TPlanPago_PciId",
                table: "TPlanPago",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TParametroLiquidacionTercero_PciId",
                table: "TParametroLiquidacionTercero",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TContrato_PciId",
                table: "TContrato",
                column: "PciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TContrato_TPci_PciId",
                table: "TContrato",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TParametroLiquidacionTercero_TPci_PciId",
                table: "TParametroLiquidacionTercero",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TPlanPago_TPci_PciId",
                table: "TPlanPago",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRelacionContable_TPci_PciId",
                table: "TRelacionContable",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TContrato_TPci_PciId",
                table: "TContrato");

            migrationBuilder.DropForeignKey(
                name: "FK_TParametroLiquidacionTercero_TPci_PciId",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropForeignKey(
                name: "FK_TPlanPago_TPci_PciId",
                table: "TPlanPago");

            migrationBuilder.DropForeignKey(
                name: "FK_TRelacionContable_TPci_PciId",
                table: "TRelacionContable");

            migrationBuilder.DropIndex(
                name: "IX_TRelacionContable_PciId",
                table: "TRelacionContable");

            migrationBuilder.DropIndex(
                name: "IX_TPlanPago_PciId",
                table: "TPlanPago");

            migrationBuilder.DropIndex(
                name: "IX_TParametroLiquidacionTercero_PciId",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropIndex(
                name: "IX_TContrato_PciId",
                table: "TContrato");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TRelacionContable");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TPlanPago");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TParametroLiquidacionTercero");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TContrato");
        }
    }
}
