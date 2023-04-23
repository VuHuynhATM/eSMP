using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v47 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            
            migrationBuilder.CreateIndex(
                name: "IX_SubCate_Specification_SpecificationID",
                table: "SubCate_Specification",
                column: "SpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_Item_Image_ImageID",
                table: "Item_Image",
                column: "ImageID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCate_Specification_Sub_Category_Sub_CategoryID",
                table: "SubCate_Specification",
                column: "Sub_CategoryID",
                principalTable: "Sub_Category",
                principalColumn: "Sub_CategoryID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            
            migrationBuilder.CreateIndex(
                name: "IX_SubCate_Specification_SpecificationID_Sub_CategoryID",
                table: "SubCate_Specification",
                columns: new[] { "SpecificationID", "Sub_CategoryID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_Image_ImageID_ItemID",
                table: "Item_Image",
                columns: new[] { "ImageID", "ItemID" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCate_Specification_Sub_Category_Sub_CategoryID",
                table: "SubCate_Specification",
                column: "Sub_CategoryID",
                principalTable: "Sub_Category",
                principalColumn: "Sub_CategoryID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
