using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v33 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderIDMOMO",
                table: "OrderBuy_Transacsion");

            migrationBuilder.RenameColumn(
                name: "MomoTransactionID",
                table: "OrderBuy_Transacsion",
                newName: "TransactionID");

            migrationBuilder.AddColumn<string>(
                name: "OrderIDGateway",
                table: "OrderBuy_Transacsion",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderIDGateway",
                table: "OrderBuy_Transacsion");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "TransactionID",
                table: "OrderBuy_Transacsion",
                newName: "MomoTransactionID");

            migrationBuilder.AddColumn<string>(
                name: "OrderIDMOMO",
                table: "OrderBuy_Transacsion",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
