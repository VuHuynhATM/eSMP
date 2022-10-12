using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageID",
                table: "Sub_Item",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Item_ImageID",
                table: "Sub_Item",
                column: "ImageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sub_Item_Image_ImageID",
                table: "Sub_Item",
                column: "ImageID",
                principalTable: "Image",
                principalColumn: "ImageID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sub_Item_Image_ImageID",
                table: "Sub_Item");

            migrationBuilder.DropIndex(
                name: "IX_Sub_Item_ImageID",
                table: "Sub_Item");

            migrationBuilder.DropColumn(
                name: "ImageID",
                table: "Sub_Item");
        }
    }
}
