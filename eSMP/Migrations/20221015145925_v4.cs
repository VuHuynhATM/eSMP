using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Feedback_Status_Feedback_StatusID",
                table: "OrderDetail");

            migrationBuilder.AlterColumn<int>(
                name: "Feedback_StatusID",
                table: "OrderDetail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FeedBack_Date",
                table: "OrderDetail",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Feedback_Status_Feedback_StatusID",
                table: "OrderDetail",
                column: "Feedback_StatusID",
                principalTable: "Feedback_Status",
                principalColumn: "Feedback_StatusID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Feedback_Status_Feedback_StatusID",
                table: "OrderDetail");

            migrationBuilder.AlterColumn<int>(
                name: "Feedback_StatusID",
                table: "OrderDetail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FeedBack_Date",
                table: "OrderDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Feedback_Status_Feedback_StatusID",
                table: "OrderDetail",
                column: "Feedback_StatusID",
                principalTable: "Feedback_Status",
                principalColumn: "Feedback_StatusID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
