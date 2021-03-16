using Microsoft.EntityFrameworkCore.Migrations;

namespace NlnrPriceDyn.DataAccess.Migrations
{
    public partial class AddedRefCountFieldToProductDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "RefsCount",
                table: "Products",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefsCount",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Products",
                nullable: false,
                defaultValue: false);
        }
    }
}
