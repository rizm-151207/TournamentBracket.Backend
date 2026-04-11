using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TournamentBracket.Api.Migrations
{
	/// <inheritdoc />
	public partial class Init : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "AspNetRoles",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
					NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
					ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoles", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUsers",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					RefreshToken = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
					RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
					UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
					NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
					Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
					NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
					EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
					PasswordHash = table.Column<string>(type: "text", nullable: true),
					SecurityStamp = table.Column<string>(type: "text", nullable: true),
					ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
					PhoneNumber = table.Column<string>(type: "text", nullable: true),
					PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
					TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
					LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
					LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
					AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUsers", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Competitions",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", maxLength: 128, nullable: false),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
					Location = table.Column<string>(type: "text", nullable: false),
					StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					Status = table.Column<int>(type: "integer", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Competitions", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Competitors",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", maxLength: 128, nullable: false),
					FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
					LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
					MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
					Gender = table.Column<bool>(type: "boolean", nullable: false),
					DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					Weight = table.Column<float>(type: "real", nullable: false),
					Rank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					Kyu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					Subject = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Competitors", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Trainers",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", maxLength: 128, nullable: false),
					FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
					LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
					MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
					Club = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
					Subject = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Trainers", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "AspNetRoleClaims",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					RoleId = table.Column<Guid>(type: "uuid", nullable: false),
					ClaimType = table.Column<string>(type: "text", nullable: true),
					ClaimValue = table.Column<string>(type: "text", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserClaims",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					UserId = table.Column<Guid>(type: "uuid", nullable: false),
					ClaimType = table.Column<string>(type: "text", nullable: true),
					ClaimValue = table.Column<string>(type: "text", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetUserClaims_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserLogins",
				columns: table => new
				{
					LoginProvider = table.Column<string>(type: "text", nullable: false),
					ProviderKey = table.Column<string>(type: "text", nullable: false),
					ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
					UserId = table.Column<Guid>(type: "uuid", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
					table.ForeignKey(
						name: "FK_AspNetUserLogins_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserRoles",
				columns: table => new
				{
					UserId = table.Column<Guid>(type: "uuid", nullable: false),
					RoleId = table.Column<Guid>(type: "uuid", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserTokens",
				columns: table => new
				{
					UserId = table.Column<Guid>(type: "uuid", nullable: false),
					LoginProvider = table.Column<string>(type: "text", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Value = table.Column<string>(type: "text", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
					table.ForeignKey(
						name: "FK_AspNetUserTokens_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Divisions",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					CompetitionId = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
					MinAge = table.Column<int>(type: "integer", nullable: true),
					MaxAge = table.Column<int>(type: "integer", nullable: true),
					MinWeight = table.Column<float>(type: "real", nullable: true),
					MaxWeight = table.Column<float>(type: "real", nullable: true),
					Gender = table.Column<bool>(type: "boolean", nullable: false),
					TournamentBracketId = table.Column<Guid>(type: "uuid", nullable: false),
					BracketType = table.Column<int>(type: "integer", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Divisions", x => x.Id);
					table.ForeignKey(
						name: "FK_Divisions_Competitions_CompetitionId",
						column: x => x.CompetitionId,
						principalTable: "Competitions",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Matches",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Index = table.Column<string>(type: "text", nullable: true),
					FirstCompetitorId = table.Column<Guid>(type: "uuid", nullable: true),
					SecondCompetitorId = table.Column<Guid>(type: "uuid", nullable: true),
					Status = table.Column<int>(type: "integer", nullable: false),
					FirstCompetitorWazari = table.Column<int>(type: "integer", nullable: false),
					FirstCompetitorKeikoku = table.Column<int>(type: "integer", nullable: false),
					FirstCompetitorChui = table.Column<int>(type: "integer", nullable: false),
					SecondCompetitorWazari = table.Column<int>(type: "integer", nullable: false),
					SecondCompetitorKeikoku = table.Column<int>(type: "integer", nullable: false),
					SecondCompetitorChui = table.Column<int>(type: "integer", nullable: false),
					Winner = table.Column<int>(type: "integer", nullable: true),
					WinReason = table.Column<int>(type: "integer", nullable: true),
					PlannedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
					StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
					EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Matches", x => x.Id);
					table.ForeignKey(
						name: "FK_Matches_Competitors_FirstCompetitorId",
						column: x => x.FirstCompetitorId,
						principalTable: "Competitors",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Matches_Competitors_SecondCompetitorId",
						column: x => x.SecondCompetitorId,
						principalTable: "Competitors",
						principalColumn: "Id");
				});

			migrationBuilder.CreateTable(
				name: "CompetitorTrainer",
				columns: table => new
				{
					CompetitorsId = table.Column<Guid>(type: "uuid", nullable: false),
					TrainersId = table.Column<Guid>(type: "uuid", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CompetitorTrainer", x => new { x.CompetitorsId, x.TrainersId });
					table.ForeignKey(
						name: "FK_CompetitorTrainer_Competitors_CompetitorsId",
						column: x => x.CompetitorsId,
						principalTable: "Competitors",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_CompetitorTrainer_Trainers_TrainersId",
						column: x => x.TrainersId,
						principalTable: "Trainers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "CompetitorDivision",
				columns: table => new
				{
					CompetitorsId = table.Column<Guid>(type: "uuid", nullable: false),
					DivisionsId = table.Column<Guid>(type: "uuid", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CompetitorDivision", x => new { x.CompetitorsId, x.DivisionsId });
					table.ForeignKey(
						name: "FK_CompetitorDivision_Competitors_CompetitorsId",
						column: x => x.CompetitorsId,
						principalTable: "Competitors",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_CompetitorDivision_Divisions_DivisionsId",
						column: x => x.DivisionsId,
						principalTable: "Divisions",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "BracketNodes",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					RoundFromFinal = table.Column<int>(type: "integer", nullable: false),
					IndexInRound = table.Column<int>(type: "integer", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					MatchId = table.Column<Guid>(type: "uuid", nullable: true),
					ParentNodeId = table.Column<Guid>(type: "uuid", nullable: true),
					BracketId = table.Column<Guid>(type: "uuid", nullable: false),
					BracketNodeId = table.Column<Guid>(type: "uuid", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_BracketNodes", x => x.Id);
					table.ForeignKey(
						name: "FK_BracketNodes_BracketNodes_BracketNodeId",
						column: x => x.BracketNodeId,
						principalTable: "BracketNodes",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_BracketNodes_Matches_MatchId",
						column: x => x.MatchId,
						principalTable: "Matches",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "SingleEliminationBrackets",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					RootId = table.Column<Guid>(type: "uuid", nullable: false),
					ThirdPlaceId = table.Column<Guid>(type: "uuid", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_SingleEliminationBrackets", x => x.Id);
					table.ForeignKey(
						name: "FK_SingleEliminationBrackets_BracketNodes_RootId",
						column: x => x.RootId,
						principalTable: "BracketNodes",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_SingleEliminationBrackets_BracketNodes_ThirdPlaceId",
						column: x => x.ThirdPlaceId,
						principalTable: "BracketNodes",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.InsertData(
				table: "AspNetRoles",
				columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
				values: new object[,]
				{
					{ new Guid("10a1a059-bbea-42a3-a2e2-c7f4c248eb11"), "3", "Organizer", "ORGANIZER" },
					{ new Guid("32018d01-edf4-4035-8921-c8cba937ef60"), "2", "Administrator", "ADMINISTRATOR" },
					{ new Guid("c44c02b2-6f22-4c39-8115-dac5072285dd"), "1", "SuperAdmin", "SUPERADMIN" }
				});

			migrationBuilder.CreateIndex(
				name: "IX_AspNetRoleClaims_RoleId",
				table: "AspNetRoleClaims",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "RoleNameIndex",
				table: "AspNetRoles",
				column: "NormalizedName",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserClaims_UserId",
				table: "AspNetUserClaims",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserLogins_UserId",
				table: "AspNetUserLogins",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserRoles_RoleId",
				table: "AspNetUserRoles",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "EmailIndex",
				table: "AspNetUsers",
				column: "NormalizedEmail");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUsers_Email",
				table: "AspNetUsers",
				column: "Email",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "UserNameIndex",
				table: "AspNetUsers",
				column: "NormalizedUserName",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_BracketNodes_BracketNodeId",
				table: "BracketNodes",
				column: "BracketNodeId");

			migrationBuilder.CreateIndex(
				name: "IX_BracketNodes_MatchId",
				table: "BracketNodes",
				column: "MatchId",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_CompetitorDivision_DivisionsId",
				table: "CompetitorDivision",
				column: "DivisionsId");

			migrationBuilder.CreateIndex(
				name: "IX_CompetitorTrainer_TrainersId",
				table: "CompetitorTrainer",
				column: "TrainersId");

			migrationBuilder.CreateIndex(
				name: "IX_Divisions_CompetitionId",
				table: "Divisions",
				column: "CompetitionId");

			migrationBuilder.CreateIndex(
				name: "IX_Matches_FirstCompetitorId",
				table: "Matches",
				column: "FirstCompetitorId");

			migrationBuilder.CreateIndex(
				name: "IX_Matches_SecondCompetitorId",
				table: "Matches",
				column: "SecondCompetitorId");

			migrationBuilder.CreateIndex(
				name: "IX_SingleEliminationBrackets_RootId",
				table: "SingleEliminationBrackets",
				column: "RootId",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_SingleEliminationBrackets_ThirdPlaceId",
				table: "SingleEliminationBrackets",
				column: "ThirdPlaceId",
				unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "AspNetRoleClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserLogins");

			migrationBuilder.DropTable(
				name: "AspNetUserRoles");

			migrationBuilder.DropTable(
				name: "AspNetUserTokens");

			migrationBuilder.DropTable(
				name: "CompetitorDivision");

			migrationBuilder.DropTable(
				name: "CompetitorTrainer");

			migrationBuilder.DropTable(
				name: "SingleEliminationBrackets");

			migrationBuilder.DropTable(
				name: "AspNetRoles");

			migrationBuilder.DropTable(
				name: "AspNetUsers");

			migrationBuilder.DropTable(
				name: "Divisions");

			migrationBuilder.DropTable(
				name: "Trainers");

			migrationBuilder.DropTable(
				name: "BracketNodes");

			migrationBuilder.DropTable(
				name: "Competitions");

			migrationBuilder.DropTable(
				name: "Matches");

			migrationBuilder.DropTable(
				name: "Competitors");
		}
	}
}
