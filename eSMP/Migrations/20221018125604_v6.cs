using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Address_AddressID",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_AddressID",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "AddressID",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pick_Address",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pick_District",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pick_Name",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pick_Province",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pick_Tel",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pick_Ward",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tel",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Pick_Address",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Pick_District",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Pick_Name",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Pick_Province",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Pick_Tel",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Pick_Ward",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Tel",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "AddressID",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Order_AddressID",
                table: "Order",
                column: "AddressID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Address_AddressID",
                table: "Order",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "AddressID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
