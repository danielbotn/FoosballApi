using Microsoft.EntityFrameworkCore.Migrations;

namespace FoosballApi.Migrations
{
    public partial class DeleteColumnMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_freehand_double_goals_users_opponent_one_id",
                table: "freehand_double_goals");

            migrationBuilder.DropForeignKey(
                name: "fk_freehand_double_goals_users_opponent_two_id",
                table: "freehand_double_goals");

            migrationBuilder.DropForeignKey(
                name: "fk_freehand_double_goals_users_teammate_id",
                table: "freehand_double_goals");

            migrationBuilder.DropIndex(
                name: "ix_freehand_double_goals_opponent_one_id",
                table: "freehand_double_goals");

            migrationBuilder.DropIndex(
                name: "ix_freehand_double_goals_opponent_two_id",
                table: "freehand_double_goals");

            migrationBuilder.DropIndex(
                name: "ix_freehand_double_goals_teammate_id",
                table: "freehand_double_goals");

            migrationBuilder.DropColumn(
                name: "opponent_one_id",
                table: "freehand_double_goals");

            migrationBuilder.DropColumn(
                name: "opponent_two_id",
                table: "freehand_double_goals");

            migrationBuilder.DropColumn(
                name: "teammate_id",
                table: "freehand_double_goals");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "opponent_one_id",
                table: "freehand_double_goals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "opponent_two_id",
                table: "freehand_double_goals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "teammate_id",
                table: "freehand_double_goals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_freehand_double_goals_opponent_one_id",
                table: "freehand_double_goals",
                column: "opponent_one_id");

            migrationBuilder.CreateIndex(
                name: "ix_freehand_double_goals_opponent_two_id",
                table: "freehand_double_goals",
                column: "opponent_two_id");

            migrationBuilder.CreateIndex(
                name: "ix_freehand_double_goals_teammate_id",
                table: "freehand_double_goals",
                column: "teammate_id");

            migrationBuilder.AddForeignKey(
                name: "fk_freehand_double_goals_users_opponent_one_id",
                table: "freehand_double_goals",
                column: "opponent_one_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_freehand_double_goals_users_opponent_two_id",
                table: "freehand_double_goals",
                column: "opponent_two_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_freehand_double_goals_users_teammate_id",
                table: "freehand_double_goals",
                column: "teammate_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
