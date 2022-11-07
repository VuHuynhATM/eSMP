using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v243 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Report");

            migrationBuilder.AddColumn<int>(
                name: "ReportStatusID",
                table: "Report",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReportStatus",
                columns: table => new
                {
                    ReportStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportStatus", x => x.ReportStatusID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportStatusID",
                table: "Report",
                column: "ReportStatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_ReportStatus_ReportStatusID",
                table: "Report",
                column: "ReportStatusID",
                principalTable: "ReportStatus",
                principalColumn: "ReportStatusID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_ReportStatus_ReportStatusID",
                table: "Report");

            migrationBuilder.DropTable(
                name: "ReportStatus");

            migrationBuilder.DropIndex(
                name: "IX_Report_ReportStatusID",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "ReportStatusID",
                table: "Report");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Report",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
