using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentBracket.Domain.Brackets.SingleEliminationBracket;

namespace TournamentBracket.Infrastructure.Models.EntityConfiguration;

public class SingleEliminationBracketConfiguration : IEntityTypeConfiguration<SingleEliminationBracket>
{
	public void Configure(EntityTypeBuilder<SingleEliminationBracket> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasOne(e => e.Root)
			.WithOne()
			.HasForeignKey<SingleEliminationBracket>(e => e.RootId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.HasOne(e => e.ThirdPlace)
			.WithOne()
			.HasForeignKey<SingleEliminationBracket>(e => e.ThirdPlaceId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Navigation(e => e.Root)
			.AutoInclude();
		builder.Navigation(e => e.ThirdPlace)
			.AutoInclude();
	}
}