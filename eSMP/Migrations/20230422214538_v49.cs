using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v49 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_System_Withdrawal_eSMP_System_SystemID",
                table: "System_Withdrawal");

            migrationBuilder.AddForeignKey(
                name: "FK_System_Withdrawal_eSMP_System_SystemID",
                table: "System_Withdrawal",
                column: "SystemID",
                principalTable: "eSMP_System",
                principalColumn: "SystemID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_System_Withdrawal_eSMP_System_SystemID",
                table: "System_Withdrawal");

            migrationBuilder.AddForeignKey(
                name: "FK_System_Withdrawal_eSMP_System_SystemID",
                table: "System_Withdrawal",
                column: "SystemID",
                principalTable: "eSMP_System",
                principalColumn: "SystemID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
