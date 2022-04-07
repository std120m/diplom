using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplom.Migrations
{
    public partial class AddBrandInfoColumnToCompaniesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrandInfo",
                table: "Companies",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandInfo",
                table: "Companies");
        }
    }
}
