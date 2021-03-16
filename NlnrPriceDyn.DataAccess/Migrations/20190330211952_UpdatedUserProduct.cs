using Microsoft.EntityFrameworkCore.Migrations;

namespace NlnrPriceDyn.DataAccess.Migrations
{
    public partial class UpdatedUserProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductNotificationDbId",
                table: "UsersProducts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersProducts_ProductNotificationDbId",
                table: "UsersProducts",
                column: "ProductNotificationDbId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersProducts_ProductNotifications_ProductNotificationDbId",
                table: "UsersProducts",
                column: "ProductNotificationDbId",
                principalTable: "ProductNotifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersProducts_ProductNotifications_ProductNotificationDbId",
                table: "UsersProducts");

            migrationBuilder.DropIndex(
                name: "IX_UsersProducts_ProductNotificationDbId",
                table: "UsersProducts");

            migrationBuilder.DropColumn(
                name: "ProductNotificationDbId",
                table: "UsersProducts");
        }
    }
}
