using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZndAlhadedStore.Migrations
{
    /// <inheritdoc />
    public partial class VerifyAuthKitMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Quotes",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "QuoteItems",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ProductCurrency",
                table: "QuoteItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "QuoteItems");

            migrationBuilder.DropColumn(
                name: "ProductCurrency",
                table: "QuoteItems");
        }
    }
}
