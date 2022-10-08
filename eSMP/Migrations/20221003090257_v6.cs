using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Specification_Sub_Category_Sub_CategoryID",
                table: "Specification");

            migrationBuilder.DropIndex(
                name: "IX_Specification_Sub_CategoryID",
                table: "Specification");

            migrationBuilder.DropColumn(
                name: "Sub_CategoryID",
                table: "Specification");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sub_CategoryID",
                table: "Specification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Specification_Sub_CategoryID",
                table: "Specification",
                column: "Sub_CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Specification_Sub_Category_Sub_CategoryID",
                table: "Specification",
                column: "Sub_CategoryID",
                principalTable: "Sub_Category",
                principalColumn: "Sub_CategoryID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
