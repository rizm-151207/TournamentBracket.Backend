using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentBracket.Domain.Divisions;

namespace TournamentBracket.Infrastructure.Models.EntityConfiguration;

public class DivisionConfiguration : IEntityTypeConfiguration<Division>
{
    public void Configure(EntityTypeBuilder<Division> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Competition)
            .WithMany(e => e.Divisions)
            .HasForeignKey(e => e.CompetitionId);
        builder.HasMany(e => e.Competitors)
            .WithMany(e => e.Divisions);

        builder.Navigation(e => e.Competitors)
            .AutoInclude();
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(e => e.Gender)
            .IsRequired()
            .HasColumnType("boolean");
        builder.Property(e => e.MinWeight)
            .HasColumnType("real");
        builder.Property(e => e.MaxWeight)
            .HasColumnType("real");
    }
}