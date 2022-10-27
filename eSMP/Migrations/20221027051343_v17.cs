using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SystemName",
                table: "eSMP_System");

            migrationBuilder.CreateTable(
                name: "System_Withdrawal",
                columns: table => new
                {
                    System_WithdrawalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemID = table.Column<int>(type: "int", nullable: false),
                    ImageID = table.Column<int>(type: "int", nullable: false),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_Withdrawal", x => x.System_WithdrawalID);
                    table.ForeignKey(
                        name: "FK_System_Withdrawal_eSMP_System_SystemID",
                        column: x => x.SystemID,
                        principalTable: "eSMP_System",
                        principalColumn: "SystemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_System_Withdrawal_SystemID",
                table: "System_Withdrawal",
                column: "SystemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "System_Withdrawal");

            migrationBuilder.AddColumn<string>(
                name: "SystemName",
                table: "eSMP_System",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
