using Microsoft.EntityFrameworkCore.Migrations;

namespace FoosballApi.Migrations
{
    public partial class newMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "current_organisation_id",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_current_organisation_id",
                table: "users",
                column: "current_organisation_id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_users_current_organisation_id",
                table: "users",
                column: "current_organisation_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_users_current_organisation_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_current_organisation_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "current_organisation_id",
                table: "users");
        }
    }
}
