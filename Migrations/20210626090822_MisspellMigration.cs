using Microsoft.EntityFrameworkCore.Migrations;

namespace FoosballApi.Migrations
{
    public partial class MisspellMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "oppenent_team_score",
                table: "freehand_double_goals",
                newName: "opponent_team_score");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "opponent_team_score",
                table: "freehand_double_goals",
                newName: "oppenent_team_score");
        }
    }
}
