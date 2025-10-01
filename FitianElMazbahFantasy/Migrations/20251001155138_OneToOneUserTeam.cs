using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitianElMazbahFantasy.Migrations
{
    /// <inheritdoc />
    public partial class OneToOneUserTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserTeams_UserId",
                table: "UserTeams");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeams_UserId",
                table: "UserTeams",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserTeams_UserId",
                table: "UserTeams");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeams_UserId",
                table: "UserTeams",
                column: "UserId");
        }
    }
}
