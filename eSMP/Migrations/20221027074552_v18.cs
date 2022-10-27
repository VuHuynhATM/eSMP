using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Withdrawal_Status",
                columns: table => new
                {
                    Withdrawal_StatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Withdrawal_Status", x => x.Withdrawal_StatusID);
                });

            migrationBuilder.CreateTable(
                name: "Store_Withdrawal",
                columns: table => new
                {
                    Store_WithdrawalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreID = table.Column<int>(type: "int", nullable: false),
                    ImageID = table.Column<int>(type: "int", nullable: true),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Withdrawal_StatusID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store_Withdrawal", x => x.Store_WithdrawalID);
                    table.ForeignKey(
                        name: "FK_Store_Withdrawal_Image_ImageID",
                        column: x => x.ImageID,
                        principalTable: "Image",
                        principalColumn: "ImageID");
                    table.ForeignKey(
                        name: "FK_Store_Withdrawal_Store_StoreID",
                        column: x => x.StoreID,
                        principalTable: "Store",
                        principalColumn: "StoreID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Store_Withdrawal_Withdrawal_Status_Withdrawal_StatusID",
                        column: x => x.Withdrawal_StatusID,
                        principalTable: "Withdrawal_Status",
                        principalColumn: "Withdrawal_StatusID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Store_Withdrawal_ImageID",
                table: "Store_Withdrawal",
                column: "ImageID");

            migrationBuilder.CreateIndex(
                name: "IX_Store_Withdrawal_StoreID",
                table: "Store_Withdrawal",
                column: "StoreID");

            migrationBuilder.CreateIndex(
                name: "IX_Store_Withdrawal_Withdrawal_StatusID",
                table: "Store_Withdrawal",
                column: "Withdrawal_StatusID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Store_Withdrawal");

            migrationBuilder.DropTable(
                name: "Withdrawal_Status");
        }
    }
}
