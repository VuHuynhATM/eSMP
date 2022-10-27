using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v171 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_System_Withdrawal_ImageID",
                table: "System_Withdrawal",
                column: "ImageID");

            migrationBuilder.AddForeignKey(
                name: "FK_System_Withdrawal_Image_ImageID",
                table: "System_Withdrawal",
                column: "ImageID",
                principalTable: "Image",
                principalColumn: "ImageID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_System_Withdrawal_Image_ImageID",
                table: "System_Withdrawal");

            migrationBuilder.DropIndex(
                name: "IX_System_Withdrawal_ImageID",
                table: "System_Withdrawal");
        }
    }
}
