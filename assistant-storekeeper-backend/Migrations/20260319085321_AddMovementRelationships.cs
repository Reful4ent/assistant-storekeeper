using Microsoft.EntityFrameworkCore.Migrations;

namespace assistant_storekeeper_backend.Migrations
{
    public partial class AddMovementRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Movements_CompanyWarehouseFromId",
                table: "Movements",
                column: "CompanyWarehouseFromId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_CompanyWarehouseToId",
                table: "Movements",
                column: "CompanyWarehouseToId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_CompanyWarehouses_CompanyWarehouseFromId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_CompanyWarehouses_CompanyWarehouseToId",
                table: "Movements");

            migrationBuilder.DropIndex(
                name: "IX_Movements_CompanyWarehouseFromId",
                table: "Movements");

            migrationBuilder.DropIndex(
                name: "IX_Movements_CompanyWarehouseToId",
                table: "Movements");
        }
    }
}
