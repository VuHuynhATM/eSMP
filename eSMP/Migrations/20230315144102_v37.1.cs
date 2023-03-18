using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v371 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataExchangeUser",
                columns: table => new
                {
                    ExchangeUserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExchangeUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangePrice = table.Column<double>(type: "float", nullable: false),
                    Create_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExchangeStatusID = table.Column<int>(type: "int", nullable: false),
                    ImageID = table.Column<int>(type: "int", nullable: true),
                    OrderID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataExchangeUser", x => x.ExchangeUserID);
                    table.ForeignKey(
                        name: "FK_DataExchangeUser_Image_ImageID",
                        column: x => x.ImageID,
                        principalTable: "Image",
                        principalColumn: "ImageID");
                    table.ForeignKey(
                        name: "FK_DataExchangeUser_Order_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "OrderID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataExchangeUser_ImageID",
                table: "DataExchangeUser",
                column: "ImageID");

            migrationBuilder.CreateIndex(
                name: "IX_DataExchangeUser_OrderID",
                table: "DataExchangeUser",
                column: "OrderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataExchangeUser");
        }
    }
}
