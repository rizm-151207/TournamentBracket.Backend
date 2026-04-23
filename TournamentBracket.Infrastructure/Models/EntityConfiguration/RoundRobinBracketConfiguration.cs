using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentBracket.Domain.Brackets.RoundRobinBracket;

namespace TournamentBracket.Infrastructure.Models.EntityConfiguration;

public class RoundRobinBracketConfiguration : IEntityTypeConfiguration<RoundRobinBracket>
{
	public void Configure(EntityTypeBuilder<RoundRobinBracket> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasMany(e => e.Matches);
		builder.Navigation(e => e.Matches)
			.AutoInclude();
		builder.HasOne(e => e.Winner);
		builder.Navigation(e => e.Winner)
			.AutoInclude();
	}
}
