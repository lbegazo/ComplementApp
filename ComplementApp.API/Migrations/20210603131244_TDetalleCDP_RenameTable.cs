using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TDetalleCDP_RenameTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TDetalleCDP_TPci_PciId",
                table: "TDetalleCDP");

            migrationBuilder.DropForeignKey(
                name: "FK_TDetalleCDP_TUsuario_UsuarioId",
                table: "TDetalleCDP");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TDetalleCDP",
                table: "TDetalleCDP");

            migrationBuilder.RenameTable(
                name: "TDetalleCDP",
                newName: "TPlanAdquisicion");

            migrationBuilder.RenameIndex(
                name: "IX_TDetalleCDP_UsuarioId",
                table: "TPlanAdquisicion",
                newName: "IX_TPlanAdquisicion_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_TDetalleCDP_PciId",
                table: "TPlanAdquisicion",
                newName: "IX_TPlanAdquisicion_PciId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TPlanAdquisicion",
                table: "TPlanAdquisicion",
                column: "DetalleCdpId");

            migrationBuilder.AddForeignKey(
                name: "FK_TPlanAdquisicion_TPci_PciId",
                table: "TPlanAdquisicion",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TPlanAdquisicion_TUsuario_UsuarioId",
                table: "TPlanAdquisicion",
                column: "UsuarioId",
                principalTable: "TUsuario",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TPlanAdquisicion_TPci_PciId",
                table: "TPlanAdquisicion");

            migrationBuilder.DropForeignKey(
                name: "FK_TPlanAdquisicion_TUsuario_UsuarioId",
                table: "TPlanAdquisicion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TPlanAdquisicion",
                table: "TPlanAdquisicion");

            migrationBuilder.RenameTable(
                name: "TPlanAdquisicion",
                newName: "TDetalleCDP");

            migrationBuilder.RenameIndex(
                name: "IX_TPlanAdquisicion_UsuarioId",
                table: "TDetalleCDP",
                newName: "IX_TDetalleCDP_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_TPlanAdquisicion_PciId",
                table: "TDetalleCDP",
                newName: "IX_TDetalleCDP_PciId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TDetalleCDP",
                table: "TDetalleCDP",
                column: "DetalleCdpId");

            migrationBuilder.AddForeignKey(
                name: "FK_TDetalleCDP_TPci_PciId",
                table: "TDetalleCDP",
                column: "PciId",
                principalTable: "TPci",
                principalColumn: "PciId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TDetalleCDP_TUsuario_UsuarioId",
                table: "TDetalleCDP",
                column: "UsuarioId",
                principalTable: "TUsuario",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
