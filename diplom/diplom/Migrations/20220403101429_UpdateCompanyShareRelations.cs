using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplom.Migrations
{
    public partial class UpdateCompanyShareRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Shares_ShareId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_ShareId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ShareId",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Shares",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shares_CompanyId",
                table: "Shares",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_Companies_CompanyId",
                table: "Shares",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shares_Companies_CompanyId",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_Shares_CompanyId",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Shares");

            migrationBuilder.AddColumn<int>(
                name: "ShareId",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ShareId",
                table: "Companies",
                column: "ShareId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Shares_ShareId",
                table: "Companies",
                column: "ShareId",
                principalTable: "Shares",
                principalColumn: "Id");
        }
    }
}
