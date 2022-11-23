using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v281 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Item_ItemID",
                table: "Notification");

            migrationBuilder.AlterColumn<int>(
                name: "ItemID",
                table: "Notification",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Notification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserID",
                table: "Notification",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Item_ItemID",
                table: "Notification",
                column: "ItemID",
                principalTable: "Item",
                principalColumn: "ItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_UserID",
                table: "Notification",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Item_ItemID",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_UserID",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_UserID",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Notification");

            migrationBuilder.AlterColumn<int>(
                name: "ItemID",
                table: "Notification",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Item_ItemID",
                table: "Notification",
                column: "ItemID",
                principalTable: "Item",
                principalColumn: "ItemID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
