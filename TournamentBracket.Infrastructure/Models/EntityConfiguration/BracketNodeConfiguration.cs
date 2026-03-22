using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentBracket.Domain.Brackets;

namespace TournamentBracket.Infrastructure.Models.EntityConfiguration;

public class BracketNodeConfiguration : IEntityTypeConfiguration<BracketNode>
{
    public void Configure(EntityTypeBuilder<BracketNode> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasMany(e => e.Children)
            .WithOne(e => e.Parent)
            .HasForeignKey(e => e.ParentNodeId);

        builder.HasOne(e => e.Match)
            .WithOne()
            .HasForeignKey<BracketNode>(e => e.MatchId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Navigation(e => e.Match)
            .AutoInclude();
    }
}