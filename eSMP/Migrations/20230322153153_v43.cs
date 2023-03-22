using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v43 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "AfterBuyService",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "AfterBuyService");
        }
    }
}
