using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v34 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Item");

            migrationBuilder.AddColumn<double>(
                name: "Discount",
                table: "Sub_Item",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "WarrantiesTime",
                table: "Sub_Item",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Sub_Item");

            migrationBuilder.DropColumn(
                name: "WarrantiesTime",
                table: "Sub_Item");

            migrationBuilder.AddColumn<double>(
                name: "Discount",
                table: "Item",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
