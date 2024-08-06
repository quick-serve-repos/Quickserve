using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickServe.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updatedbIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantityMax",
                table: "Ingredient",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityMax",
                table: "Ingredient");
        }
    }
}
