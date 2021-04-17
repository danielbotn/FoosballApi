using System;
using FoosballApi.Models.Leagues;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FoosballApi.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "organisations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organisations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    email = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "leagues",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    type_of_league = table.Column<LeagueType>(type: "league_type", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    up_to = table.Column<int>(type: "integer", nullable: false),
                    organisation_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_leagues", x => x.id);
                    table.ForeignKey(
                        name: "fk_leagues_organisations_organisation_id",
                        column: x => x.organisation_id,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "freehand_matches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    player_one_id = table.Column<int>(type: "integer", nullable: false),
                    player_two_id = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    player_one_score = table.Column<int>(type: "integer", nullable: false),
                    player_two_score = table.Column<int>(type: "integer", nullable: false),
                    up_to = table.Column<int>(type: "integer", nullable: false),
                    game_finished = table.Column<bool>(type: "boolean", nullable: false),
                    game_paused = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_freehand_matches", x => x.id);
                    table.ForeignKey(
                        name: "fk_freehand_matches_users_player_one_id",
                        column: x => x.player_one_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_freehand_matches_users_player_two_id",
                        column: x => x.player_two_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "organisation_list",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    organisation_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organisation_list", x => x.id);
                    table.ForeignKey(
                        name: "fk_organisation_list_organisations_organisation_id",
                        column: x => x.organisation_id,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_organisation_list_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "verifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    verification_token = table.Column<string>(type: "text", nullable: true),
                    password_reset_token = table.Column<string>(type: "text", nullable: true),
                    password_reset_token_expires = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    has_verified = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_verifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "league_players",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    league_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_league_players", x => x.id);
                    table.ForeignKey(
                        name: "fk_league_players_leagues_league_id",
                        column: x => x.league_id,
                        principalTable: "leagues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_league_players_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "freehand_goals",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    time_of_goal = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    scored_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    oponent_id = table.Column<int>(type: "integer", nullable: false),
                    scored_by_score = table.Column<int>(type: "integer", nullable: false),
                    oponent_score = table.Column<int>(type: "integer", nullable: false),
                    winner_goal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_freehand_goals", x => x.id);
                    table.ForeignKey(
                        name: "fk_freehand_goals_freehand_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "freehand_matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_freehand_goals_users_oponent_id",
                        column: x => x.oponent_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_freehand_goals_users_scored_by_user_id",
                        column: x => x.scored_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_freehand_goals_match_id",
                table: "freehand_goals",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "ix_freehand_goals_oponent_id",
                table: "freehand_goals",
                column: "oponent_id");

            migrationBuilder.CreateIndex(
                name: "ix_freehand_goals_scored_by_user_id",
                table: "freehand_goals",
                column: "scored_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_freehand_matches_player_one_id",
                table: "freehand_matches",
                column: "player_one_id");

            migrationBuilder.CreateIndex(
                name: "ix_freehand_matches_player_two_id",
                table: "freehand_matches",
                column: "player_two_id");

            migrationBuilder.CreateIndex(
                name: "ix_league_players_league_id",
                table: "league_players",
                column: "league_id");

            migrationBuilder.CreateIndex(
                name: "ix_league_players_user_id",
                table: "league_players",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_leagues_organisation_id",
                table: "leagues",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "ix_organisation_list_organisation_id",
                table: "organisation_list",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "ix_organisation_list_user_id",
                table: "organisation_list",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_verifications_user_id",
                table: "verifications",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "freehand_goals");

            migrationBuilder.DropTable(
                name: "league_players");

            migrationBuilder.DropTable(
                name: "organisation_list");

            migrationBuilder.DropTable(
                name: "verifications");

            migrationBuilder.DropTable(
                name: "freehand_matches");

            migrationBuilder.DropTable(
                name: "leagues");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "organisations");
        }
    }
}
