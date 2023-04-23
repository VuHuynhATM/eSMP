using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSMP.Migrations
{
    public partial class v46 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCate_Specification_Specification_SpecificationID",
                table: "SubCate_Specification");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCate_Specification_Specification_SpecificationID",
                table: "SubCate_Specification",
                column: "SpecificationID",
                principalTable: "Specification",
                principalColumn: "SpecificationID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCate_Specification_Specification_SpecificationID",
                table: "SubCate_Specification");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCate_Specification_Specification_SpecificationID",
                table: "SubCate_Specification",
                column: "SpecificationID",
                principalTable: "Specification",
                principalColumn: "SpecificationID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
