using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentBracket.Domain.Competitions;

namespace TournamentBracket.Infrastructure.Models.EntityConfiguration;

public class CompetitionConfiguration : IEntityTypeConfiguration<Competition>
{
    public void Configure(EntityTypeBuilder<Competition> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasMany(e => e.Competitors)
            .WithMany(e => e.Competitions);
        builder.HasMany(e => e.Divisions)
            .WithOne(e => e.Competition);
        
        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(128);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(e => e.Description)
            .IsRequired();
        builder.Property(e => e.StartDateTime)
            .IsRequired();
        builder.Property(e => e.Status)
            .IsRequired();
    }
}