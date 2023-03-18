using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v391 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Firsr_FeeShip",
                table: "AfterBuyService");

            migrationBuilder.RenameColumn(
                name: "SendToCustemrTime",
                table: "AfterBuyService",
                newName: "estimated_pick_time");

            migrationBuilder.RenameColumn(
                name: "Firsr_User_Province",
                table: "AfterBuyService",
                newName: "User_Province");

            migrationBuilder.RenameColumn(
                name: "Firsr_Create_Date",
                table: "AfterBuyService",
                newName: "Create_Date");

            migrationBuilder.AddColumn<double>(
                name: "FeeShip",
                table: "AfterBuyService",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "estimated_deliver_time",
                table: "AfterBuyService",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeeShip",
                table: "AfterBuyService");

            migrationBuilder.DropColumn(
                name: "estimated_deliver_time",
                table: "AfterBuyService");

            migrationBuilder.RenameColumn(
                name: "estimated_pick_time",
                table: "AfterBuyService",
                newName: "SendToCustemrTime");

            migrationBuilder.RenameColumn(
                name: "User_Province",
                table: "AfterBuyService",
                newName: "Firsr_User_Province");

            migrationBuilder.RenameColumn(
                name: "Create_Date",
                table: "AfterBuyService",
                newName: "Firsr_Create_Date");

            migrationBuilder.AddColumn<double>(
                name: "Firsr_FeeShip",
                table: "AfterBuyService",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
