using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_User_Status_StatusID",
                table: "User");

            migrationBuilder.DropTable(
                name: "User_Status");

            migrationBuilder.DropIndex(
                name: "IX_User_StatusID",
                table: "User");

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "User");

            migrationBuilder.CreateTable(
                name: "User_Status",
                columns: table => new
                {
                    StatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Status", x => x.StatusID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_StatusID",
                table: "User",
                column: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_User_User_Status_StatusID",
                table: "User",
                column: "StatusID",
                principalTable: "User_Status",
                principalColumn: "StatusID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
