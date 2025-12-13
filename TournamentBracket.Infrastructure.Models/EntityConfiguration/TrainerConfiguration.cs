using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Infrastructure.Models.EntityConfiguration;

public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
{
    public void Configure(EntityTypeBuilder<Trainer> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasMany(e => e.Competitors)
            .WithMany(e => e.Trainers);
        
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
        builder.Property(e => e.Club)
            .HasMaxLength(200);
        builder.Property(e => e.Subject)
            .HasMaxLength(100);
    }
}