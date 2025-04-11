using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarketEssatto.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "StockQuotes",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					TickerSymbol = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
					CompanyName = table.Column<string>(type: "TEXT", nullable: false),
					FetchDate = table.Column<DateTime>(type: "TEXT", nullable: false),
					Price = table.Column<decimal>(type: "TEXT", nullable: false),
					IsActiveTrading = table.Column<bool>(type: "INTEGER", nullable: false),
					LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_StockQuotes", x => x.Id);
				});

			migrationBuilder.CreateIndex(
				name: "IX_StockQuotes_TickerSymbol",
				table: "StockQuotes",
				column: "TickerSymbol",
				unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "StockQuotes");
		}
	}
}
