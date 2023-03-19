using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v40 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeeShip",
                table: "AfterBuyService",
                newName: "FeeShipSercond");

            migrationBuilder.AddColumn<double>(
                name: "FeeShipFisrt",
                table: "AfterBuyService",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeeShipFisrt",
                table: "AfterBuyService");

            migrationBuilder.RenameColumn(
                name: "FeeShipSercond",
                table: "AfterBuyService",
                newName: "FeeShip");
        }
    }
}
