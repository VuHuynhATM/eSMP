using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Actice_Date",
                table: "Store",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MomoTransactionID",
                table: "Store",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Actice_Date",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "MomoTransactionID",
                table: "Store");
        }
    }
}
