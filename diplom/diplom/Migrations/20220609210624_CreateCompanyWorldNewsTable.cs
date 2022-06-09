using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplom.Migrations
{
    public partial class CreateCompanyWorldNewsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Company_World_News",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    WorldNewsId = table.Column<int>(type: "int", nullable: false),
                    Influence = table.Column<long>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company_World_News", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Company_World_News_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Company_World_News_World_News_WorldNewsId",
                        column: x => x.WorldNewsId,
                        principalTable: "World_News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Company_World_News_CompanyId",
                table: "Company_World_News",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_World_News_WorldNewsId",
                table: "Company_World_News",
                column: "WorldNewsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Company_World_News");
        }
    }
}
