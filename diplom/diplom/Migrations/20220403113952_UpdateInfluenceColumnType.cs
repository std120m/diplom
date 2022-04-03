using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplom.Migrations
{
    public partial class UpdateInfluenceColumnType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Influence",
                table: "Company_World_News",
                type: "double",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Influence",
                table: "Company_World_News",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");
        }
    }
}
