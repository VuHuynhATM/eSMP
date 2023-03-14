using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v36 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataExchangeStore",
                columns: table => new
                {
                    ExchangeStoreID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExchangeStoreName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangePrice = table.Column<double>(type: "float", nullable: false),
                    Create_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExchangeStatusID = table.Column<int>(type: "int", nullable: false),
                    ImageID = table.Column<int>(type: "int", nullable: true),
                    OrderID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataExchangeStore", x => x.ExchangeStoreID);
                    table.ForeignKey(
                        name: "FK_DataExchangeStore_Image_ImageID",
                        column: x => x.ImageID,
                        principalTable: "Image",
                        principalColumn: "ImageID");
                    table.ForeignKey(
                        name: "FK_DataExchangeStore_Order_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "OrderID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataExchangeStore_ImageID",
                table: "DataExchangeStore",
                column: "ImageID");

            migrationBuilder.CreateIndex(
                name: "IX_DataExchangeStore_OrderID",
                table: "DataExchangeStore",
                column: "OrderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataExchangeStore");
        }
    }
}
