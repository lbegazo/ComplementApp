using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDocumentoGneral_AddPci : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TDocumentoOrdenPago",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TDocumentoObligacion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TDocumentoCompromiso",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PciId",
                table: "TDocumentoCdp",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TDocumentoOrdenPago_PciId",
                table: "TDocumentoOrdenPago",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TDocumentoObligacion_PciId",
                table: "TDocumentoObligacion",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TDocumentoCompromiso_PciId",
                table: "TDocumentoCompromiso",
                column: "PciId");

            migrationBuilder.CreateIndex(
                name: "IX_TDocumentoCdp_PciId",
                table: "TDocumentoCdp",
                column: "PciId");

            migrationBuilder.AddForeignKey(
                name: "FK_TDocumentoCdp_TPci_PciId",
                table: "TDocumentoCdp",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TDocumentoCompromiso_TPci_PciId",
                table: "TDocumentoCompromiso",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TDocumentoObligacion_TPci_PciId",
                table: "TDocumentoObligacion",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TDocumentoOrdenPago_TPci_PciId",
                table: "TDocumentoOrdenPago",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TDocumentoCdp_TPci_PciId",
                table: "TDocumentoCdp");

            migrationBuilder.DropForeignKey(
                name: "FK_TDocumentoCompromiso_TPci_PciId",
                table: "TDocumentoCompromiso");

            migrationBuilder.DropForeignKey(
                name: "FK_TDocumentoObligacion_TPci_PciId",
                table: "TDocumentoObligacion");

            migrationBuilder.DropForeignKey(
                name: "FK_TDocumentoOrdenPago_TPci_PciId",
                table: "TDocumentoOrdenPago");

            migrationBuilder.DropIndex(
                name: "IX_TDocumentoOrdenPago_PciId",
                table: "TDocumentoOrdenPago");

            migrationBuilder.DropIndex(
                name: "IX_TDocumentoObligacion_PciId",
                table: "TDocumentoObligacion");

            migrationBuilder.DropIndex(
                name: "IX_TDocumentoCompromiso_PciId",
                table: "TDocumentoCompromiso");

            migrationBuilder.DropIndex(
                name: "IX_TDocumentoCdp_PciId",
                table: "TDocumentoCdp");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TDocumentoOrdenPago");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TDocumentoObligacion");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TDocumentoCompromiso");

            migrationBuilder.DropColumn(
                name: "PciId",
                table: "TDocumentoCdp");
        }
    }
}
