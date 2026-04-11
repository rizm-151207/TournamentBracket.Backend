using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Infrastructure.Models.EntityConfiguration;

public class MatchProcessConfiguration : IEntityTypeConfiguration<MatchProcess>
{
	public void Configure(EntityTypeBuilder<MatchProcess> builder)
	{
		builder.HasKey(e => e.MatchId);
		builder.ToTable(MatchConfiguration.TableName);
	}
}