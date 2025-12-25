using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentBracket.Domain.Users;

namespace TournamentBracket.Infrastructure.Models.EntityConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(e => e.Email)
            .IsUnique();
        builder.Property(e => e.RefreshToken)
            .HasMaxLength(256);
    }
}