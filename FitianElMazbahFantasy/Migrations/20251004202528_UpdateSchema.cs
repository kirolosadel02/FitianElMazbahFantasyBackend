using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitianElMazbahFantasy.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MatchweekId",
                table: "Fixtures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Matchweeks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeekNumber = table.Column<int>(type: "int", nullable: false),
                    DeadlineDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matchweeks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTeamMatchweekPoints",
                columns: table => new
                {
                    UserTeamId = table.Column<int>(type: "int", nullable: false),
                    MatchweekId = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Goals = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Assists = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CleanSheets = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    YellowCards = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RedCards = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Saves = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Penalties = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTeamMatchweekPoints", x => new { x.UserTeamId, x.MatchweekId });
                    table.ForeignKey(
                        name: "FK_UserTeamMatchweekPoints_Matchweeks_MatchweekId",
                        column: x => x.MatchweekId,
                        principalTable: "Matchweeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTeamMatchweekPoints_UserTeams_UserTeamId",
                        column: x => x.UserTeamId,
                        principalTable: "UserTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTeamSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserTeamId = table.Column<int>(type: "int", nullable: false),
                    MatchweekId = table.Column<int>(type: "int", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SnapshotDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTeamSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTeamSnapshots_Matchweeks_MatchweekId",
                        column: x => x.MatchweekId,
                        principalTable: "Matchweeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTeamSnapshots_UserTeams_UserTeamId",
                        column: x => x.UserTeamId,
                        principalTable: "UserTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTeamSnapshotPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SnapshotId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    PlayerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTeamSnapshotPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTeamSnapshotPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTeamSnapshotPlayers_UserTeamSnapshots_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "UserTeamSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_MatchweekId",
                table: "Fixtures",
                column: "MatchweekId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchweeks_DeadlineDate",
                table: "Matchweeks",
                column: "DeadlineDate");

            migrationBuilder.CreateIndex(
                name: "IX_Matchweeks_IsActive",
                table: "Matchweeks",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Matchweeks_IsCompleted",
                table: "Matchweeks",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_Matchweeks_WeekNumber",
                table: "Matchweeks",
                column: "WeekNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTeamMatchweekPoints_MatchweekId",
                table: "UserTeamMatchweekPoints",
                column: "MatchweekId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeamMatchweekPoints_Points",
                table: "UserTeamMatchweekPoints",
                column: "Points");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeamMatchweekPoints_UserTeamId",
                table: "UserTeamMatchweekPoints",
                column: "UserTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeamSnapshotPlayers_PlayerId",
                table: "UserTeamSnapshotPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeamSnapshotPlayers_SnapshotId",
                table: "UserTeamSnapshotPlayers",
                column: "SnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeamSnapshotPlayers_SnapshotId_PlayerId",
                table: "UserTeamSnapshotPlayers",
                columns: new[] { "SnapshotId", "PlayerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTeamSnapshots_MatchweekId",
                table: "UserTeamSnapshots",
                column: "MatchweekId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeamSnapshots_UserTeamId",
                table: "UserTeamSnapshots",
                column: "UserTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeamSnapshots_UserTeamId_MatchweekId",
                table: "UserTeamSnapshots",
                columns: new[] { "UserTeamId", "MatchweekId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Fixtures_Matchweeks_MatchweekId",
                table: "Fixtures",
                column: "MatchweekId",
                principalTable: "Matchweeks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fixtures_Matchweeks_MatchweekId",
                table: "Fixtures");

            migrationBuilder.DropTable(
                name: "UserTeamMatchweekPoints");

            migrationBuilder.DropTable(
                name: "UserTeamSnapshotPlayers");

            migrationBuilder.DropTable(
                name: "UserTeamSnapshots");

            migrationBuilder.DropTable(
                name: "Matchweeks");

            migrationBuilder.DropIndex(
                name: "IX_Fixtures_MatchweekId",
                table: "Fixtures");

            migrationBuilder.DropColumn(
                name: "MatchweekId",
                table: "Fixtures");
        }
    }
}
