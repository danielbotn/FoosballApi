using Microsoft.EntityFrameworkCore.Migrations;

namespace FoosballApi.Migrations
{
    public partial class typeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "organisations");

            migrationBuilder.AddColumn<int>(
                name: "organisation_type",
                table: "organisations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "organisation_type",
                table: "organisations");

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "organisations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
