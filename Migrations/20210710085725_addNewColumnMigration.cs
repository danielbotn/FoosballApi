using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FoosballApi.Migrations
{
    public partial class addNewColumnMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "how_many_rounds",
                table: "leagues",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "single_league_matches_query",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    player_one = table.Column<int>(type: "integer", nullable: false),
                    player_two = table.Column<int>(type: "integer", nullable: false),
                    league_id = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    player_one_score = table.Column<int>(type: "integer", nullable: true),
                    player_two_score = table.Column<int>(type: "integer", nullable: true),
                    match_started = table.Column<bool>(type: "boolean", nullable: true),
                    match_ended = table.Column<bool>(type: "boolean", nullable: true),
                    match_paused = table.Column<bool>(type: "boolean", nullable: true),
                    organisation_id = table.Column<int>(type: "integer", nullable: false),
                    player_one_first_name = table.Column<string>(type: "text", nullable: true),
                    player_one_last_name = table.Column<string>(type: "text", nullable: true),
                    player_two_first_name = table.Column<string>(type: "text", nullable: true),
                    player_two_last_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "single_league_matches_query");

            migrationBuilder.DropColumn(
                name: "how_many_rounds",
                table: "leagues");
        }
    }
}
