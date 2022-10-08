using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sub_Item_Image");

            migrationBuilder.CreateTable(
                name: "Item_Image",
                columns: table => new
                {
                    Item_ImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    ImageID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item_Image", x => x.Item_ImageID);
                    table.ForeignKey(
                        name: "FK_Item_Image_Image_ImageID",
                        column: x => x.ImageID,
                        principalTable: "Image",
                        principalColumn: "ImageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Item_Image_Item_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Item",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_Image_ImageID",
                table: "Item_Image",
                column: "ImageID");

            migrationBuilder.CreateIndex(
                name: "IX_Item_Image_ItemID",
                table: "Item_Image",
                column: "ItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Item_Image");

            migrationBuilder.CreateTable(
                name: "Sub_Item_Image",
                columns: table => new
                {
                    Sub_Item_ImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageID = table.Column<int>(type: "int", nullable: false),
                    Sub_ItemID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_Item_Image", x => x.Sub_Item_ImageID);
                    table.ForeignKey(
                        name: "FK_Sub_Item_Image_Image_ImageID",
                        column: x => x.ImageID,
                        principalTable: "Image",
                        principalColumn: "ImageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sub_Item_Image_Sub_Item_Sub_ItemID",
                        column: x => x.Sub_ItemID,
                        principalTable: "Sub_Item",
                        principalColumn: "Sub_ItemID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Item_Image_ImageID",
                table: "Sub_Item_Image",
                column: "ImageID");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Item_Image_Sub_ItemID",
                table: "Sub_Item_Image",
                column: "Sub_ItemID");
        }
    }
}
