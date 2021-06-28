using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FoosballApi.Migrations
{
    public partial class NewTableMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "freehand_double_goals",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    time_of_goal = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    double_match_id = table.Column<int>(type: "integer", nullable: false),
                    scored_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    opponent_one_id = table.Column<int>(type: "integer", nullable: false),
                    opponent_two_id = table.Column<int>(type: "integer", nullable: false),
                    scorer_team_score = table.Column<int>(type: "integer", nullable: false),
                    teammate_id = table.Column<int>(type: "integer", nullable: false),
                    oppenent_team_score = table.Column<int>(type: "integer", nullable: false),
                    winner_goal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_freehand_double_goals", x => x.id);
                    table.ForeignKey(
                        name: "fk_freehand_double_goals_freehand_double_matches_double_match_id",
                        column: x => x.double_match_id,
                        principalTable: "freehand_double_matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_freehand_double_goals_users_opponent_one_id",
                        column: x => x.opponent_one_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_freehand_double_goals_users_opponent_two_id",
                        column: x => x.opponent_two_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_freehand_double_goals_users_scored_by_user_id",
                        column: x => x.scored_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_freehand_double_goals_users_teammate_id",
                        column: x => x.teammate_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_freehand_double_goals_double_match_id",
                table: "freehand_double_goals",
                column: "double_match_id");

            migrationBuilder.CreateIndex(
                name: "ix_freehand_double_goals_opponent_one_id",
                table: "freehand_double_goals",
                column: "opponent_one_id");

            migrationBuilder.CreateIndex(
                name: "ix_freehand_double_goals_opponent_two_id",
                table: "freehand_double_goals",
                column: "opponent_two_id");

            migrationBuilder.CreateIndex(
                name: "ix_freehand_double_goals_scored_by_user_id",
                table: "freehand_double_goals",
                column: "scored_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_freehand_double_goals_teammate_id",
                table: "freehand_double_goals",
                column: "teammate_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "freehand_double_goals");
        }
    }
}
