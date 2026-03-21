using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentBracket.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTatami : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tatami",
                table: "Divisions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TatamiCount",
                table: "Competitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tatami",
                table: "Divisions");

            migrationBuilder.DropColumn(
                name: "TatamiCount",
                table: "Competitions");
        }
    }
}
