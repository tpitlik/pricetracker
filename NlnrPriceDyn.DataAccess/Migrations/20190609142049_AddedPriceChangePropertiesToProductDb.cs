using Microsoft.EntityFrameworkCore.Migrations;

namespace NlnrPriceDyn.DataAccess.Migrations
{
    public partial class AddedPriceChangePropertiesToProductDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "MaxPriceChange",
                table: "Products",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MeanPriceChange",
                table: "Products",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MinPriceChange",
                table: "Products",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxPriceChange",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MeanPriceChange",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MinPriceChange",
                table: "Products");
        }
    }
}
