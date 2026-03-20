using Microsoft.EntityFrameworkCore.Migrations;

namespace assistant_storekeeper_backend.Migrations
{
    public partial class DeleteCascadeAndSetNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_CompanyWarehouses_CompanyWarehouseFromId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_CompanyWarehouses_CompanyWarehouseToId",
                table: "Movements");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyWarehouseNomenclatures_CompanyWarehouses_CompanyWare~",
                table: "CompanyWarehouseNomenclatures",
                column: "CompanyWarehouseId",
                principalTable: "CompanyWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyWarehouseNomenclatures_Nomenclatures_NomenclatureId",
                table: "CompanyWarehouseNomenclatures",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementNomenclatures_Movements_MovementId",
                table: "MovementNomenclatures",
                column: "MovementId",
                principalTable: "Movements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementNomenclatures_Nomenclatures_NomenclatureId",
                table: "MovementNomenclatures",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_CompanyWarehouses_CompanyWarehouseFromId",
                table: "Movements",
                column: "CompanyWarehouseFromId",
                principalTable: "CompanyWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_CompanyWarehouses_CompanyWarehouseToId",
                table: "Movements",
                column: "CompanyWarehouseToId",
                principalTable: "CompanyWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_CompanyWarehouses_CompanyWarehouseFromId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_CompanyWarehouses_CompanyWarehouseToId",
                table: "Movements");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_CompanyWarehouses_CompanyWarehouseFromId",
                table: "Movements",
                column: "CompanyWarehouseFromId",
                principalTable: "CompanyWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_CompanyWarehouses_CompanyWarehouseToId",
                table: "Movements",
                column: "CompanyWarehouseToId",
                principalTable: "CompanyWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
