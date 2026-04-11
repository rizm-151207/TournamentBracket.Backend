using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Infrastructure.Models.EntityConfiguration;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
	public const string TableName = "Matches";
	public void Configure(EntityTypeBuilder<Match> builder)
	{
		builder.HasKey(e => e.Id);
		builder.ToTable(TableName);

		builder.HasOne(e => e.FirstCompetitor)
			.WithMany();
		builder.HasOne(e => e.SecondCompetitor)
			.WithMany();
		builder.HasOne(e => e.MatchProcess)
			.WithOne()
			.HasForeignKey<MatchProcess>(e => e.MatchId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Navigation(e => e.FirstCompetitor)
			.AutoInclude();
		builder.Navigation(e => e.SecondCompetitor)
			.AutoInclude();
		builder.Navigation(e => e.MatchProcess)
			.AutoInclude();
	}
}