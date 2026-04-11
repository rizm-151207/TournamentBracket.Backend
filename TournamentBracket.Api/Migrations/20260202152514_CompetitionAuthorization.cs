using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentBracket.Api.Migrations
{
	/// <inheritdoc />
	public partial class CompetitionAuthorization : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "OwnerEmail",
				table: "Competitions",
				type: "character varying(100)",
				maxLength: 100,
				nullable: false,
				defaultValue: "");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "OwnerEmail",
				table: "Competitions");
		}
	}
}
