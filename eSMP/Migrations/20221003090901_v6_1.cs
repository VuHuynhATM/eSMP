using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v6_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubCate_Specification",
                columns: table => new
                {
                    SubCate_SpecificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sub_CategoryID = table.Column<int>(type: "int", nullable: false),
                    SpecificationID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCate_Specification", x => x.SubCate_SpecificationID);
                    table.ForeignKey(
                        name: "FK_SubCate_Specification_Specification_SpecificationID",
                        column: x => x.SpecificationID,
                        principalTable: "Specification",
                        principalColumn: "SpecificationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubCate_Specification_Sub_Category_Sub_CategoryID",
                        column: x => x.Sub_CategoryID,
                        principalTable: "Sub_Category",
                        principalColumn: "Sub_CategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubCate_Specification_SpecificationID",
                table: "SubCate_Specification",
                column: "SpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_SubCate_Specification_Sub_CategoryID",
                table: "SubCate_Specification",
                column: "Sub_CategoryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubCate_Specification");
        }
    }
}
