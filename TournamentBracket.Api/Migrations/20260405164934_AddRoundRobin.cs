using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentBracket.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRoundRobin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoundRobinBracketId",
                table: "Matches",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoundRobinBrackets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundRobinBrackets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_RoundRobinBracketId",
                table: "Matches",
                column: "RoundRobinBracketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_RoundRobinBrackets_RoundRobinBracketId",
                table: "Matches",
                column: "RoundRobinBracketId",
                principalTable: "RoundRobinBrackets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_RoundRobinBrackets_RoundRobinBracketId",
                table: "Matches");

            migrationBuilder.DropTable(
                name: "RoundRobinBrackets");

            migrationBuilder.DropIndex(
                name: "IX_Matches_RoundRobinBracketId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "RoundRobinBracketId",
                table: "Matches");
        }
    }
}
