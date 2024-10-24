using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShoppingCartMvcUI.Migrations
{
    /// <inheritdoc />
    public partial class _3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProdutoId1",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "CartDetail",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_ProdutoId1",
                table: "Order",
                column: "ProdutoId1");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetail_OrderId",
                table: "CartDetail",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetail_Order_OrderId",
                table: "CartDetail",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Produto_ProdutoId1",
                table: "Order",
                column: "ProdutoId1",
                principalTable: "Produto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetail_Order_OrderId",
                table: "CartDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Produto_ProdutoId1",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ProdutoId1",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_CartDetail_OrderId",
                table: "CartDetail");

            migrationBuilder.DropColumn(
                name: "ProdutoId1",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "CartDetail");
        }
    }
}
