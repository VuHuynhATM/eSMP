using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v45 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_Image_Image_ImageID",
                table: "Item_Image");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Image_Image_ImageID",
                table: "Item_Image",
                column: "ImageID",
                principalTable: "Image",
                principalColumn: "ImageID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_Image_Image_ImageID",
                table: "Item_Image");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Image_Image_ImageID",
                table: "Item_Image",
                column: "ImageID",
                principalTable: "Image",
                principalColumn: "ImageID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
