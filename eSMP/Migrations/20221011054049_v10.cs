using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Sub_Item");

            migrationBuilder.AddColumn<int>(
                name: "SubItem_StatusID",
                table: "Sub_Item",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SubItem_Status",
                columns: table => new
                {
                    SubItem_StatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubItem_Status", x => x.SubItem_StatusID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Item_SubItem_StatusID",
                table: "Sub_Item",
                column: "SubItem_StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sub_Item_SubItem_Status_SubItem_StatusID",
                table: "Sub_Item",
                column: "SubItem_StatusID",
                principalTable: "SubItem_Status",
                principalColumn: "SubItem_StatusID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sub_Item_SubItem_Status_SubItem_StatusID",
                table: "Sub_Item");

            migrationBuilder.DropTable(
                name: "SubItem_Status");

            migrationBuilder.DropIndex(
                name: "IX_Sub_Item_SubItem_StatusID",
                table: "Sub_Item");

            migrationBuilder.DropColumn(
                name: "SubItem_StatusID",
                table: "Sub_Item");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Sub_Item",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
