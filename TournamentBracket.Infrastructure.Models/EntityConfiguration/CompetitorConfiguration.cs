using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Infrastructure.Models.EntityConfiguration;

public class CompetitorConfiguration : IEntityTypeConfiguration<Competitor>
{
    public void Configure(EntityTypeBuilder<Competitor> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasMany(e => e.Trainers)
            .WithMany(e => e.Competitors);

        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(128);
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.MiddleName)
            .HasMaxLength(100);
        builder.Property(e => e.DateOfBirth)
            .IsRequired()
            .HasColumnType("datetime");
        builder.Property(e => e.Gender)
            .IsRequired()
            .HasColumnType("boolean");
        builder.Property(e => e.Kyu)
            .HasMaxLength(50);
        builder.Property(e => e.Rank)
            .HasMaxLength(50);
        builder.Property(e => e.Subject)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.Weight)
            .IsRequired()
            .HasColumnType("real");
    }
}