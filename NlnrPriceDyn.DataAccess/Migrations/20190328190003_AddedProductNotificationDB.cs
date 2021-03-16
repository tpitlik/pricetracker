using Microsoft.EntityFrameworkCore.Migrations;

namespace NlnrPriceDyn.DataAccess.Migrations
{
    public partial class AddedProductNotificationDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    LowPriceLimit = table.Column<float>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ProductId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductNotifications_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductNotifications_ProductId",
                table: "ProductNotifications",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductNotifications");
        }
    }
}
