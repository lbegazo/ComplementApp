using Microsoft.EntityFrameworkCore.Migrations;

namespace ComplementApp.API.Migrations
{
    public partial class TCargaObligacion_AddFuenteIdSituacionIdRecursoPreId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FuenteFinanciacion",
                table: "TCargaObligacion");

            migrationBuilder.DropColumn(
                name: "RecursoPresupuestal",
                table: "TCargaObligacion");

            migrationBuilder.DropColumn(
                name: "RubroDescripcion",
                table: "TCargaObligacion");

            migrationBuilder.DropColumn(
                name: "RubroIdentificacion",
                table: "TCargaObligacion");

            migrationBuilder.DropColumn(
                name: "SituacionFondo",
                table: "TCargaObligacion");

            migrationBuilder.AddColumn<int>(
                name: "FuenteFinanciacionId",
                table: "TCargaObligacion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecursoPresupuestalId",
                table: "TCargaObligacion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SituacionFondoId",
                table: "TCargaObligacion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TCargaObligacion_FuenteFinanciacionId",
                table: "TCargaObligacion",
                column: "FuenteFinanciacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TCargaObligacion_RecursoPresupuestalId",
                table: "TCargaObligacion",
                column: "RecursoPresupuestalId");

            migrationBuilder.CreateIndex(
                name: "IX_TCargaObligacion_SituacionFondoId",
                table: "TCargaObligacion",
                column: "SituacionFondoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCargaObligacion_TFuenteFinanciacion_FuenteFinanciacionId",
                table: "TCargaObligacion",
                column: "FuenteFinanciacionId",
                principalTable: "TFuenteFinanciacion",
                principalColumn: "FuenteFinanciacionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TCargaObligacion_TRecursoPresupuestal_RecursoPresupuestalId",
                table: "TCargaObligacion",
                column: "RecursoPresupuestalId",
                principalTable: "TRecursoPresupuestal",
                principalColumn: "RecursoPresupuestalId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TCargaObligacion_TSituacionFondo_SituacionFondoId",
                table: "TCargaObligacion",
                column: "SituacionFondoId",
                principalTable: "TSituacionFondo",
                principalColumn: "SituacionFondoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCargaObligacion_TFuenteFinanciacion_FuenteFinanciacionId",
                table: "TCargaObligacion");

            migrationBuilder.DropForeignKey(
                name: "FK_TCargaObligacion_TRecursoPresupuestal_RecursoPresupuestalId",
                table: "TCargaObligacion");

            migrationBuilder.DropForeignKey(
                name: "FK_TCargaObligacion_TSituacionFondo_SituacionFondoId",
                table: "TCargaObligacion");

            migrationBuilder.DropIndex(
                name: "IX_TCargaObligacion_FuenteFinanciacionId",
                table: "TCargaObligacion");

            migrationBuilder.DropIndex(
                name: "IX_TCargaObligacion_RecursoPresupuestalId",
                table: "TCargaObligacion");

            migrationBuilder.DropIndex(
                name: "IX_TCargaObligacion_SituacionFondoId",
                table: "TCargaObligacion");

            migrationBuilder.DropColumn(
                name: "FuenteFinanciacionId",
                table: "TCargaObligacion");

            migrationBuilder.DropColumn(
                name: "RecursoPresupuestalId",
                table: "TCargaObligacion");

            migrationBuilder.DropColumn(
                name: "SituacionFondoId",
                table: "TCargaObligacion");

            migrationBuilder.AddColumn<string>(
                name: "FuenteFinanciacion",
                table: "TCargaObligacion",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecursoPresupuestal",
                table: "TCargaObligacion",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RubroDescripcion",
                table: "TCargaObligacion",
                type: "VARCHAR(250)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RubroIdentificacion",
                table: "TCargaObligacion",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SituacionFondo",
                table: "TCargaObligacion",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "");
        }
    }
}
