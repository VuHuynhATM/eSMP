using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v81 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderRefund_Transaction",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Create_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResultCode = table.Column<int>(type: "int", nullable: false),
                    MomoTransactionID = table.Column<long>(type: "bigint", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRefund_Transaction", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderRefund_Transaction_Order_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipStatus",
                columns: table => new
                {
                    Status_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status_Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipStatus", x => x.Status_ID);
                });

            migrationBuilder.CreateTable(
                name: "ShipOrder",
                columns: table => new
                {
                    ShipOrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    Status_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Reason_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LabelID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipOrder", x => x.ShipOrderID);
                    table.ForeignKey(
                        name: "FK_ShipOrder_Order_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipOrder_ShipStatus_Status_ID",
                        column: x => x.Status_ID,
                        principalTable: "ShipStatus",
                        principalColumn: "Status_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderRefund_Transaction_OrderID",
                table: "OrderRefund_Transaction",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ShipOrder_OrderID",
                table: "ShipOrder",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ShipOrder_Status_ID",
                table: "ShipOrder",
                column: "Status_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderRefund_Transaction");

            migrationBuilder.DropTable(
                name: "ShipOrder");

            migrationBuilder.DropTable(
                name: "ShipStatus");
        }
    }
}
