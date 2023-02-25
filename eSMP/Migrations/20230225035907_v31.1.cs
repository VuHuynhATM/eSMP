using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v311 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecificationSuggest");

            migrationBuilder.AddColumn<string>(
                name: "SpecificationSuggests",
                table: "Specification",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecificationSuggests",
                table: "Specification");

            migrationBuilder.CreateTable(
                name: "SpecificationSuggest",
                columns: table => new
                {
                    SpecificationSuggestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SuggestValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationSuggest", x => x.SpecificationSuggestID);
                    table.ForeignKey(
                        name: "FK_SpecificationSuggest_Specification_SpecificationID",
                        column: x => x.SpecificationID,
                        principalTable: "Specification",
                        principalColumn: "SpecificationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationSuggest_SpecificationID",
                table: "SpecificationSuggest",
                column: "SpecificationID");
        }
    }
}
