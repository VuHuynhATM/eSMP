using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubCate_Specification_SpecificationID",
                table: "SubCate_Specification");

            migrationBuilder.DropIndex(
                name: "IX_Specification_Value_SpecificationID",
                table: "Specification_Value");

            migrationBuilder.DropIndex(
                name: "IX_Model_Item_ItemID",
                table: "Model_Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_Image_ImageID",
                table: "Item_Image");

            migrationBuilder.CreateTable(
                name: "Feedback_Status",
                columns: table => new
                {
                    Feedback_StatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback_Status", x => x.Feedback_StatusID);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Create_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPay = table.Column<bool>(type: "bit", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    AddressID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Order_Address_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Address",
                        principalColumn: "AddressID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PricePurchase = table.Column<double>(type: "float", nullable: false),
                    DiscountPurchase = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Feedback_Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feedback_Rate = table.Column<double>(type: "float", nullable: true),
                    FeedBack_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sub_ItemID = table.Column<int>(type: "int", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    Feedback_StatusID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Feedback_Status_Feedback_StatusID",
                        column: x => x.Feedback_StatusID,
                        principalTable: "Feedback_Status",
                        principalColumn: "Feedback_StatusID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Sub_Item_Sub_ItemID",
                        column: x => x.Sub_ItemID,
                        principalTable: "Sub_Item",
                        principalColumn: "Sub_ItemID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Feedback_Image",
                columns: table => new
                {
                    Feedback_ImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDetailID = table.Column<int>(type: "int", nullable: false),
                    ImageID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback_Image", x => x.Feedback_ImageID);
                    table.ForeignKey(
                        name: "FK_Feedback_Image_Image_ImageID",
                        column: x => x.ImageID,
                        principalTable: "Image",
                        principalColumn: "ImageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedback_Image_OrderDetail_OrderDetailID",
                        column: x => x.OrderDetailID,
                        principalTable: "OrderDetail",
                        principalColumn: "OrderDetailID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubCate_Specification_SpecificationID_Sub_CategoryID",
                table: "SubCate_Specification",
                columns: new[] { "SpecificationID", "Sub_CategoryID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specification_Value_SpecificationID_ItemID",
                table: "Specification_Value",
                columns: new[] { "SpecificationID", "ItemID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Model_Item_ItemID_Brand_ModelID",
                table: "Model_Item",
                columns: new[] { "ItemID", "Brand_ModelID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_Image_ImageID_ItemID",
                table: "Item_Image",
                columns: new[] { "ImageID", "ItemID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_Image_ImageID_OrderDetailID",
                table: "Feedback_Image",
                columns: new[] { "ImageID", "OrderDetailID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_Image_OrderDetailID",
                table: "Feedback_Image",
                column: "OrderDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_Order_AddressID",
                table: "Order",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserID",
                table: "Order",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_Feedback_StatusID",
                table: "OrderDetail",
                column: "Feedback_StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderID",
                table: "OrderDetail",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_Sub_ItemID",
                table: "OrderDetail",
                column: "Sub_ItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedback_Image");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "Feedback_Status");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropIndex(
                name: "IX_SubCate_Specification_SpecificationID_Sub_CategoryID",
                table: "SubCate_Specification");

            migrationBuilder.DropIndex(
                name: "IX_Specification_Value_SpecificationID_ItemID",
                table: "Specification_Value");

            migrationBuilder.DropIndex(
                name: "IX_Model_Item_ItemID_Brand_ModelID",
                table: "Model_Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_Image_ImageID_ItemID",
                table: "Item_Image");

            migrationBuilder.CreateIndex(
                name: "IX_SubCate_Specification_SpecificationID",
                table: "SubCate_Specification",
                column: "SpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_Specification_Value_SpecificationID",
                table: "Specification_Value",
                column: "SpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_Model_Item_ItemID",
                table: "Model_Item",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Item_Image_ImageID",
                table: "Item_Image",
                column: "ImageID");
        }
    }
}
