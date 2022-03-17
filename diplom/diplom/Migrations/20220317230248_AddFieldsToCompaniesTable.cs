using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplom.Migrations
{
    public partial class AddFieldsToCompaniesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Website = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EnterpriseValue = table.Column<long>(type: "bigint", nullable: true),
                    ForwardPE = table.Column<long>(type: "bigint", nullable: true),
                    ProfitMargins = table.Column<double>(type: "double", nullable: true),
                    FloatShares = table.Column<long>(type: "bigint", nullable: true),
                    FullTimeEmployees = table.Column<int>(type: "int", nullable: true),
                    SharesOutstanding = table.Column<long>(type: "bigint", nullable: true),
                    SharesShort = table.Column<long>(type: "bigint", nullable: true),
                    SharesShortPriorMonth = table.Column<long>(type: "bigint", nullable: true),
                    ShortRatio = table.Column<double>(type: "double", nullable: true),
                    ShortPercentOfFloat = table.Column<double>(type: "double", nullable: true),
                    BookValuePerShare = table.Column<double>(type: "double", nullable: true),
                    PriceToBook = table.Column<double>(type: "double", nullable: true),
                    NetIncomeToCommon = table.Column<long>(type: "bigint", nullable: true),
                    TrailingEps = table.Column<double>(type: "double", nullable: true),
                    EnterpriseToRevenue = table.Column<double>(type: "double", nullable: true),
                    EnterpriseToEbitda = table.Column<double>(type: "double", nullable: true),
                    Week52Change = table.Column<double>(type: "double", nullable: true),
                    SandP52WeekChange = table.Column<double>(type: "double", nullable: true),
                    TotalCash = table.Column<long>(type: "bigint", nullable: true),
                    TotalCashPerShare = table.Column<double>(type: "double", nullable: true),
                    Ebitda = table.Column<long>(type: "bigint", nullable: true),
                    TotalDebt = table.Column<long>(type: "bigint", nullable: true),
                    CurrentRatio = table.Column<double>(type: "double", nullable: true),
                    Revenue = table.Column<long>(type: "bigint", nullable: true),
                    DebtToEquity = table.Column<double>(type: "double", nullable: true),
                    RevenuePerShare = table.Column<double>(type: "double", nullable: true),
                    ReturnOnAssets = table.Column<double>(type: "double", nullable: true),
                    ReturnOnEquity = table.Column<double>(type: "double", nullable: true),
                    GrossProfits = table.Column<long>(type: "bigint", nullable: true),
                    FreeCashflow = table.Column<long>(type: "bigint", nullable: true),
                    OperatingCashflow = table.Column<long>(type: "bigint", nullable: true),
                    RevenueGrowth = table.Column<double>(type: "double", nullable: true),
                    OperatingMargins = table.Column<double>(type: "double", nullable: true),
                    ShareId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_Shares_ShareId",
                        column: x => x.ShareId,
                        principalTable: "Shares",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ShareId",
                table: "Companies",
                column: "ShareId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
