using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplom.Migrations
{
    public partial class s : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_Events_Companies_CompanyId",
                table: "Company_Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_Filings_Companies_CompanyId",
                table: "Company_Filings");

            migrationBuilder.DropTable(
                name: "Company_World_News");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company_Filings",
                table: "Company_Filings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company_Events",
                table: "Company_Events");

            migrationBuilder.RenameTable(
                name: "Company_Filings",
                newName: "Company_Filings");

            migrationBuilder.RenameTable(
                name: "Company_Events",
                newName: "Company_Events");

            migrationBuilder.RenameIndex(
                name: "IX_Company_Filings_CompanyId",
                table: "Company_Filings",
                newName: "IX_Company_Filings_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Company_Events_CompanyId",
                table: "Company_Events",
                newName: "IX_Company_Events_CompanyId");

            migrationBuilder.AlterColumn<int>(
                name: "ShareType",
                table: "Shares",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company_Filings",
                table: "Company_Filings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company_Events",
                table: "Company_Events",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Events_Companies_CompanyId",
                table: "Company_Events",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Filings_Companies_CompanyId",
                table: "Company_Filings",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_Events_Companies_CompanyId",
                table: "Company_Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_Filings_Companies_CompanyId",
                table: "Company_Filings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company_Filings",
                table: "Company_Filings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company_Events",
                table: "Company_Events");

            migrationBuilder.RenameTable(
                name: "Company_Filings",
                newName: "CompanyFilings");

            migrationBuilder.RenameTable(
                name: "Company_Events",
                newName: "CompanyEvents");

            migrationBuilder.RenameIndex(
                name: "IX_Company_Filings_CompanyId",
                table: "CompanyFilings",
                newName: "IX_CompanyFilings_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Company_Events_CompanyId",
                table: "CompanyEvents",
                newName: "IX_CompanyEvents_CompanyId");

            migrationBuilder.AlterColumn<int>(
                name: "ShareType",
                table: "Shares",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyFilings",
                table: "CompanyFilings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyEvents",
                table: "CompanyEvents",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CompanyWorldNews",
                columns: table => new
                {
                    CompaniesId = table.Column<int>(type: "int", nullable: false),
                    WorldNewsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyWorldNews", x => new { x.CompaniesId, x.WorldNewsId });
                    table.ForeignKey(
                        name: "FK_CompanyWorldNews_Companies_CompaniesId",
                        column: x => x.CompaniesId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyWorldNews_World_News_WorldNewsId",
                        column: x => x.WorldNewsId,
                        principalTable: "World_News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyWorldNews_WorldNewsId",
                table: "CompanyWorldNews",
                column: "WorldNewsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyEvents_Companies_CompanyId",
                table: "CompanyEvents",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyFilings_Companies_CompanyId",
                table: "CompanyFilings",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }
    }
}
