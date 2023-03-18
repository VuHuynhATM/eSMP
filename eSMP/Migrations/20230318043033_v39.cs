using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v39 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipOrder_Order_OrderID",
                table: "ShipOrder");

            migrationBuilder.AlterColumn<int>(
                name: "OrderID",
                table: "ShipOrder",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AfterBuyServiceID",
                table: "ShipOrder",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AfterBuyServiceID",
                table: "DataExchangeUser",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AfterBuyServiceID",
                table: "DataExchangeStore",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AfterBuyService",
                columns: table => new
                {
                    AfterBuyServiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firsr_Create_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Firsr_FeeShip = table.Column<double>(type: "float", nullable: false),
                    Firsr_User_Province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_Ward = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_Tel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Store_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Store_Tel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Store_Province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Store_District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Store_Ward = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Store_Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pick_Time = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefundPrice = table.Column<double>(type: "float", nullable: true),
                    ServicestatusID = table.Column<int>(type: "int", nullable: false),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    SendToCustemrTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AfterBuyService", x => x.AfterBuyServiceID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDetail",
                columns: table => new
                {
                    ServiceDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDetailID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AfterBuyServiceID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDetail", x => x.ServiceDetailID);
                    table.ForeignKey(
                        name: "FK_ServiceDetail_AfterBuyService_AfterBuyServiceID",
                        column: x => x.AfterBuyServiceID,
                        principalTable: "AfterBuyService",
                        principalColumn: "AfterBuyServiceID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceDetail_OrderDetail_OrderDetailID",
                        column: x => x.OrderDetailID,
                        principalTable: "OrderDetail",
                        principalColumn: "OrderDetailID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipOrder_AfterBuyServiceID",
                table: "ShipOrder",
                column: "AfterBuyServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_DataExchangeUser_AfterBuyServiceID",
                table: "DataExchangeUser",
                column: "AfterBuyServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_DataExchangeStore_AfterBuyServiceID",
                table: "DataExchangeStore",
                column: "AfterBuyServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetail_AfterBuyServiceID",
                table: "ServiceDetail",
                column: "AfterBuyServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetail_OrderDetailID",
                table: "ServiceDetail",
                column: "OrderDetailID");

            migrationBuilder.AddForeignKey(
                name: "FK_DataExchangeStore_AfterBuyService_AfterBuyServiceID",
                table: "DataExchangeStore",
                column: "AfterBuyServiceID",
                principalTable: "AfterBuyService",
                principalColumn: "AfterBuyServiceID");

            migrationBuilder.AddForeignKey(
                name: "FK_DataExchangeUser_AfterBuyService_AfterBuyServiceID",
                table: "DataExchangeUser",
                column: "AfterBuyServiceID",
                principalTable: "AfterBuyService",
                principalColumn: "AfterBuyServiceID");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipOrder_AfterBuyService_AfterBuyServiceID",
                table: "ShipOrder",
                column: "AfterBuyServiceID",
                principalTable: "AfterBuyService",
                principalColumn: "AfterBuyServiceID");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipOrder_Order_OrderID",
                table: "ShipOrder",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "OrderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataExchangeStore_AfterBuyService_AfterBuyServiceID",
                table: "DataExchangeStore");

            migrationBuilder.DropForeignKey(
                name: "FK_DataExchangeUser_AfterBuyService_AfterBuyServiceID",
                table: "DataExchangeUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipOrder_AfterBuyService_AfterBuyServiceID",
                table: "ShipOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipOrder_Order_OrderID",
                table: "ShipOrder");

            migrationBuilder.DropTable(
                name: "ServiceDetail");

            migrationBuilder.DropTable(
                name: "AfterBuyService");

            migrationBuilder.DropIndex(
                name: "IX_ShipOrder_AfterBuyServiceID",
                table: "ShipOrder");

            migrationBuilder.DropIndex(
                name: "IX_DataExchangeUser_AfterBuyServiceID",
                table: "DataExchangeUser");

            migrationBuilder.DropIndex(
                name: "IX_DataExchangeStore_AfterBuyServiceID",
                table: "DataExchangeStore");

            migrationBuilder.DropColumn(
                name: "AfterBuyServiceID",
                table: "ShipOrder");

            migrationBuilder.DropColumn(
                name: "AfterBuyServiceID",
                table: "DataExchangeUser");

            migrationBuilder.DropColumn(
                name: "AfterBuyServiceID",
                table: "DataExchangeStore");

            migrationBuilder.AlterColumn<int>(
                name: "OrderID",
                table: "ShipOrder",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipOrder_Order_OrderID",
                table: "ShipOrder",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
