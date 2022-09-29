using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "Address",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "Address",
                newName: "Latitude");

            migrationBuilder.RenameColumn(
                name: "context",
                table: "Address",
                newName: "Context");

            migrationBuilder.AddColumn<int>(
                name: "Pick_date",
                table: "Store",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pick_date",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "Address");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Address",
                newName: "longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Address",
                newName: "latitude");

            migrationBuilder.RenameColumn(
                name: "Context",
                table: "Address",
                newName: "context");
        }
    }
}
