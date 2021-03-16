using Microsoft.EntityFrameworkCore.Migrations;

namespace NlnrPriceDyn.DataAccess.Migrations
{
    public partial class DeleteProductFromProductNoticationDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductNotifications_Products_ProductId",
                table: "ProductNotifications");

            migrationBuilder.DropIndex(
                name: "IX_ProductNotifications_ProductId",
                table: "ProductNotifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductNotifications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "ProductNotifications",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductNotifications_ProductId",
                table: "ProductNotifications",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductNotifications_Products_ProductId",
                table: "ProductNotifications",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
