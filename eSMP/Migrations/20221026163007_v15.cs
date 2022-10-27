using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "eSMP_System",
                columns: table => new
                {
                    SystemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Asset = table.Column<double>(type: "float", nullable: false),
                    Commission_Precent = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eSMP_System", x => x.SystemID);
                });

            migrationBuilder.CreateTable(
                name: "OrderSystem_Transaction",
                columns: table => new
                {
                    OrderSystem_TransactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderStore_TransactionID = table.Column<int>(type: "int", nullable: false),
                    SystemID = table.Column<int>(type: "int", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSystem_Transaction", x => x.OrderSystem_TransactionID);
                    table.ForeignKey(
                        name: "FK_OrderSystem_Transaction_eSMP_System_SystemID",
                        column: x => x.SystemID,
                        principalTable: "eSMP_System",
                        principalColumn: "SystemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderSystem_Transaction_OrderStore_Transaction_OrderStore_TransactionID",
                        column: x => x.OrderStore_TransactionID,
                        principalTable: "OrderStore_Transaction",
                        principalColumn: "OrderStore_TransactionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderSystem_Transaction_OrderStore_TransactionID",
                table: "OrderSystem_Transaction",
                column: "OrderStore_TransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSystem_Transaction_SystemID",
                table: "OrderSystem_Transaction",
                column: "SystemID");
        }
    }
}
