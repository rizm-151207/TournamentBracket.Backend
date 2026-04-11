using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentBracket.Api.Migrations
{
	/// <inheritdoc />
	public partial class CompetitorSoftDelete : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_BracketNodes_BracketNodes_BracketNodeId",
				table: "BracketNodes");

			migrationBuilder.DropIndex(
				name: "IX_BracketNodes_BracketNodeId",
				table: "BracketNodes");

			migrationBuilder.DropColumn(
				name: "BracketNodeId",
				table: "BracketNodes");

			migrationBuilder.AddColumn<DateTime>(
				name: "DeletedAt",
				table: "Competitors",
				type: "timestamp with time zone",
				nullable: true);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Competitors",
				type: "boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.CreateIndex(
				name: "IX_BracketNodes_ParentNodeId",
				table: "BracketNodes",
				column: "ParentNodeId");

			migrationBuilder.AddForeignKey(
				name: "FK_BracketNodes_BracketNodes_ParentNodeId",
				table: "BracketNodes",
				column: "ParentNodeId",
				principalTable: "BracketNodes",
				principalColumn: "Id");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_BracketNodes_BracketNodes_ParentNodeId",
				table: "BracketNodes");

			migrationBuilder.DropIndex(
				name: "IX_BracketNodes_ParentNodeId",
				table: "BracketNodes");

			migrationBuilder.DropColumn(
				name: "DeletedAt",
				table: "Competitors");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Competitors");

			migrationBuilder.AddColumn<Guid>(
				name: "BracketNodeId",
				table: "BracketNodes",
				type: "uuid",
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_BracketNodes_BracketNodeId",
				table: "BracketNodes",
				column: "BracketNodeId");

			migrationBuilder.AddForeignKey(
				name: "FK_BracketNodes_BracketNodes_BracketNodeId",
				table: "BracketNodes",
				column: "BracketNodeId",
				principalTable: "BracketNodes",
				principalColumn: "Id");
		}
	}
}
