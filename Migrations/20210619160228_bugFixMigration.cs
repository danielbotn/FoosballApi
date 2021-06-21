using Microsoft.EntityFrameworkCore.Migrations;

namespace FoosballApi.Migrations
{
    public partial class bugFixMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_users_current_organisation_id",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "fk_users_organisations_current_organisation_id",
                table: "users",
                column: "current_organisation_id",
                principalTable: "organisations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_organisations_current_organisation_id",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "fk_users_users_current_organisation_id",
                table: "users",
                column: "current_organisation_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
