using Microsoft.EntityFrameworkCore.Migrations;

namespace assistant_storekeeper_backend.Migrations
{
    public partial class AddCompanyWarehouseNomenclature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MovementNomenclatures_NomenclatureId",
                table: "MovementNomenclatures",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyWarehouseNomenclatures_CompanyWarehouseId",
                table: "CompanyWarehouseNomenclatures",
                column: "CompanyWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyWarehouseNomenclatures_NomenclatureId",
                table: "CompanyWarehouseNomenclatures",
                column: "NomenclatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyWarehouseNomenclatures_CompanyWarehouses_CompanyWare~",
                table: "CompanyWarehouseNomenclatures",
                column: "CompanyWarehouseId",
                principalTable: "CompanyWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyWarehouseNomenclatures_Nomenclatures_NomenclatureId",
                table: "CompanyWarehouseNomenclatures",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementNomenclatures_Movements_MovementId",
                table: "MovementNomenclatures",
                column: "MovementId",
                principalTable: "Movements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementNomenclatures_Nomenclatures_NomenclatureId",
                table: "MovementNomenclatures",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyWarehouseNomenclatures_CompanyWarehouses_CompanyWare~",
                table: "CompanyWarehouseNomenclatures");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyWarehouseNomenclatures_Nomenclatures_NomenclatureId",
                table: "CompanyWarehouseNomenclatures");

            migrationBuilder.DropForeignKey(
                name: "FK_MovementNomenclatures_Movements_MovementId",
                table: "MovementNomenclatures");

            migrationBuilder.DropForeignKey(
                name: "FK_MovementNomenclatures_Nomenclatures_NomenclatureId",
                table: "MovementNomenclatures");

            migrationBuilder.DropIndex(
                name: "IX_MovementNomenclatures_NomenclatureId",
                table: "MovementNomenclatures");

            migrationBuilder.DropIndex(
                name: "IX_CompanyWarehouseNomenclatures_CompanyWarehouseId",
                table: "CompanyWarehouseNomenclatures");

            migrationBuilder.DropIndex(
                name: "IX_CompanyWarehouseNomenclatures_NomenclatureId",
                table: "CompanyWarehouseNomenclatures");
        }
    }
}
