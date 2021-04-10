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
            // migrationBuilder.CreateTable(
            //     name: "organisations",
            //     columns: table => new
            //     {
            //         id = table.Column<int>(type: "integer", nullable: false)
            //             .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            //         name = table.Column<string>(type: "text", nullable: false),
            //         created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //         type = table.Column<string>(type: "text", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("pk_organisations", x => x.id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "users",
            //     columns: table => new
            //     {
            //         id = table.Column<int>(type: "integer", nullable: false)
            //             .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            //         email = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
            //         password = table.Column<string>(type: "text", nullable: false),
            //         first_name = table.Column<string>(type: "text", nullable: false),
            //         last_name = table.Column<string>(type: "text", nullable: false),
            //         created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("pk_users", x => x.id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "leagues",
            //     columns: table => new
            //     {
            //         id = table.Column<int>(type: "integer", nullable: false)
            //             .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            //         name = table.Column<string>(type: "text", nullable: false),
            //         type_of_league = table.Column<LeagueType>(type: "league_type", nullable: false),
            //         created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //         up_to = table.Column<int>(type: "integer", nullable: false),
            //         organisation_id = table.Column<int>(type: "integer", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("pk_leagues", x => x.id);
            //         table.ForeignKey(
            //             name: "fk_leagues_organisations_organisation_id",
            //             column: x => x.organisation_id,
            //             principalTable: "organisations",
            //             principalColumn: "id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "organisation_list",
            //     columns: table => new
            //     {
            //         id = table.Column<int>(type: "integer", nullable: false)
            //             .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            //         organisation_id = table.Column<int>(type: "integer", nullable: false),
            //         user_id = table.Column<int>(type: "integer", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("pk_organisation_list", x => x.id);
            //         table.ForeignKey(
            //             name: "fk_organisation_list_organisations_organisation_id",
            //             column: x => x.organisation_id,
            //             principalTable: "organisations",
            //             principalColumn: "id",
            //             onDelete: ReferentialAction.Cascade);
            //         table.ForeignKey(
            //             name: "fk_organisation_list_users_user_id",
            //             column: x => x.user_id,
            //             principalTable: "users",
            //             principalColumn: "id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "verifications",
            //     columns: table => new
            //     {
            //         id = table.Column<int>(type: "integer", nullable: false)
            //             .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            //         user_id = table.Column<int>(type: "integer", nullable: false),
            //         verification_token = table.Column<string>(type: "text", nullable: true),
            //         password_reset_token = table.Column<string>(type: "text", nullable: true),
            //         password_reset_token_expires = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            //         has_verified = table.Column<bool>(type: "boolean", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("pk_verifications", x => x.id);
            //         table.ForeignKey(
            //             name: "fk_verifications_users_user_id",
            //             column: x => x.user_id,
            //             principalTable: "users",
            //             principalColumn: "id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

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

            migrationBuilder.CreateIndex(
                name: "ix_league_players_league_id",
                table: "league_players",
                column: "league_id");

            migrationBuilder.CreateIndex(
                name: "ix_league_players_user_id",
                table: "league_players",
                column: "user_id");

            // migrationBuilder.CreateIndex(
            //     name: "ix_leagues_organisation_id",
            //     table: "leagues",
            //     column: "organisation_id");

            // migrationBuilder.CreateIndex(
            //     name: "ix_organisation_list_organisation_id",
            //     table: "organisation_list",
            //     column: "organisation_id");

            // migrationBuilder.CreateIndex(
            //     name: "ix_organisation_list_user_id",
            //     table: "organisation_list",
            //     column: "user_id");

            // migrationBuilder.CreateIndex(
            //     name: "ix_verifications_user_id",
            //     table: "verifications",
            //     column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "league_players");

            migrationBuilder.DropTable(
                name: "organisation_list");

            migrationBuilder.DropTable(
                name: "verifications");

            migrationBuilder.DropTable(
                name: "leagues");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "organisations");
        }
    }
}
