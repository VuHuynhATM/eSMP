using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v241 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Report",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Report_UserID",
                table: "Report",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_User_UserID",
                table: "Report",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_User_UserID",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_UserID",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Report");
        }
    }
}
