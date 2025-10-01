using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitianElMazbahFantasy.Migrations
{
    /// <inheritdoc />
    public partial class RemovePlayerPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Players");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Players",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0.0m);
        }
    }
}
