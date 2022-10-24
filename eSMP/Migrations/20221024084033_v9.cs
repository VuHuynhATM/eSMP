using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPay",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "OrderStatusID",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    OrderStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StausName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatus", x => x.OrderStatusID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderStatusID",
                table: "Order",
                column: "OrderStatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_OrderStatus_OrderStatusID",
                table: "Order",
                column: "OrderStatusID",
                principalTable: "OrderStatus",
                principalColumn: "OrderStatusID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_OrderStatus_OrderStatusID",
                table: "Order");

            migrationBuilder.DropTable(
                name: "OrderStatus");

            migrationBuilder.DropIndex(
                name: "IX_Order_OrderStatusID",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderStatusID",
                table: "Order");

            migrationBuilder.AddColumn<bool>(
                name: "IsPay",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
