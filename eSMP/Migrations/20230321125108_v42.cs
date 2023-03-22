using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v42 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "BankSupport");

            migrationBuilder.DropColumn(
                name: "BankID",
                table: "Store_Withdrawal");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Store_Withdrawal",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Store_Withdrawal");

            migrationBuilder.AddColumn<int>(
                name: "BankID",
                table: "Store_Withdrawal",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BankSupport",
                columns: table => new
                {
                    BankID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankAvatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankSupport", x => x.BankID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Store_Withdrawal_BankID",
                table: "Store_Withdrawal",
                column: "BankID");
        }
    }
}
