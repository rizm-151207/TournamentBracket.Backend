using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentBracket.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWinnerToRoundRobin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WinnerId",
                table: "RoundRobinBrackets",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoundRobinBrackets_WinnerId",
                table: "RoundRobinBrackets",
                column: "WinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoundRobinBrackets_Competitors_WinnerId",
                table: "RoundRobinBrackets",
                column: "WinnerId",
                principalTable: "Competitors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoundRobinBrackets_Competitors_WinnerId",
                table: "RoundRobinBrackets");

            migrationBuilder.DropIndex(
                name: "IX_RoundRobinBrackets_WinnerId",
                table: "RoundRobinBrackets");

            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "RoundRobinBrackets");
        }
    }
}
