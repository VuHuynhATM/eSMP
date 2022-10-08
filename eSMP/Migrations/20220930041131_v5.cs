using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Store_UserID",
                table: "Store");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Sub_Category",
                newName: "Sub_CategoryID");

            migrationBuilder.AddColumn<string>(
                name: "Sub_categoryName",
                table: "Sub_Category",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ItemStatus",
                columns: table => new
                {
                    Item_StatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemStatus", x => x.Item_StatusID);
                });

            migrationBuilder.CreateTable(
                name: "Specification",
                columns: table => new
                {
                    SpecificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sub_CategoryID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specification", x => x.SpecificationID);
                    table.ForeignKey(
                        name: "FK_Specification_Sub_Category_Sub_CategoryID",
                        column: x => x.Sub_CategoryID,
                        principalTable: "Sub_Category",
                        principalColumn: "Sub_CategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    ItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<float>(type: "real", nullable: false),
                    Create_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoreID = table.Column<int>(type: "int", nullable: false),
                    Sub_CategoryID = table.Column<int>(type: "int", nullable: false),
                    Item_StatusID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.ItemID);
                    table.ForeignKey(
                        name: "FK_Item_ItemStatus_Item_StatusID",
                        column: x => x.Item_StatusID,
                        principalTable: "ItemStatus",
                        principalColumn: "Item_StatusID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Item_Store_StoreID",
                        column: x => x.StoreID,
                        principalTable: "Store",
                        principalColumn: "StoreID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Item_Sub_Category_Sub_CategoryID",
                        column: x => x.Sub_CategoryID,
                        principalTable: "Sub_Category",
                        principalColumn: "Sub_CategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Specification_Value",
                columns: table => new
                {
                    Specification_ValueID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecificationID = table.Column<int>(type: "int", nullable: false),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specification_Value", x => x.Specification_ValueID);
                    table.ForeignKey(
                        name: "FK_Specification_Value_Item_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Item",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Specification_Value_Specification_SpecificationID",
                        column: x => x.SpecificationID,
                        principalTable: "Specification",
                        principalColumn: "SpecificationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sub_Item",
                columns: table => new
                {
                    Sub_ItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sub_ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_Item", x => x.Sub_ItemID);
                    table.ForeignKey(
                        name: "FK_Sub_Item_Item_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Item",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sub_Item_Image",
                columns: table => new
                {
                    Sub_Item_ImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sub_ItemID = table.Column<int>(type: "int", nullable: false),
                    ImageID = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_Store_UserID",
                table: "Store",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_Item_StatusID",
                table: "Item",
                column: "Item_StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Item_StoreID",
                table: "Item",
                column: "StoreID");

            migrationBuilder.CreateIndex(
                name: "IX_Item_Sub_CategoryID",
                table: "Item",
                column: "Sub_CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Specification_Sub_CategoryID",
                table: "Specification",
                column: "Sub_CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Specification_Value_ItemID",
                table: "Specification_Value",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Specification_Value_SpecificationID",
                table: "Specification_Value",
                column: "SpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Item_ItemID",
                table: "Sub_Item",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Item_Image_ImageID",
                table: "Sub_Item_Image",
                column: "ImageID");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Item_Image_Sub_ItemID",
                table: "Sub_Item_Image",
                column: "Sub_ItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Specification_Value");

            migrationBuilder.DropTable(
                name: "Sub_Item_Image");

            migrationBuilder.DropTable(
                name: "Specification");

            migrationBuilder.DropTable(
                name: "Sub_Item");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "ItemStatus");

            migrationBuilder.DropIndex(
                name: "IX_Store_UserID",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "Sub_categoryName",
                table: "Sub_Category");

            migrationBuilder.RenameColumn(
                name: "Sub_CategoryID",
                table: "Sub_Category",
                newName: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_Store_UserID",
                table: "Store",
                column: "UserID");
        }
    }
}
