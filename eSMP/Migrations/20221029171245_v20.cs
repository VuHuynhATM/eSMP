using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Context",
                table: "Store_Withdrawal",
                newName: "OwnerBankCart");

            migrationBuilder.AddColumn<int>(
                name: "BankID",
                table: "Store_Withdrawal",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NumBankCart",
                table: "Store_Withdrawal",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BankSupport",
                columns: table => new
                {
                    BankID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankAvatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
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

            migrationBuilder.AddForeignKey(
                name: "FK_Store_Withdrawal_BankSupport_BankID",
                table: "Store_Withdrawal",
                column: "BankID",
                principalTable: "BankSupport",
                principalColumn: "BankID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Store_Withdrawal_BankSupport_BankID",
                table: "Store_Withdrawal");

            migrationBuilder.DropTable(
                name: "BankSupport");

            migrationBuilder.DropIndex(
                name: "IX_Store_Withdrawal_BankID",
                table: "Store_Withdrawal");

            migrationBuilder.DropColumn(
                name: "BankID",
                table: "Store_Withdrawal");

            migrationBuilder.DropColumn(
                name: "NumBankCart",
                table: "Store_Withdrawal");

            migrationBuilder.RenameColumn(
                name: "OwnerBankCart",
                table: "Store_Withdrawal",
                newName: "Context");
        }
    }
}
