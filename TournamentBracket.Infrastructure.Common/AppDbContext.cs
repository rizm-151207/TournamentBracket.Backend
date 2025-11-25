using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TournamentBracket.Domain.Users;

namespace TournamentBracket.Infrastructure.Common;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
        base.Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(entity =>
        {
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.MiddleName)
                .HasMaxLength(100);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasIndex(e => e.Email)
                .IsUnique();
        }) ;

        AddDefaultRoles(builder);
    }

    private void AddDefaultRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole<Guid>>(entity =>
        {
            entity.HasData([
                new IdentityRole<Guid>
                    { Id = Guid.NewGuid(), Name = "SuperAdmin", NormalizedName = "SUPERADMIN", ConcurrencyStamp = "1" },
                new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(), Name = "Administrator", NormalizedName = "ADMINISTRATOR",
                    ConcurrencyStamp = "2"
                },
                new IdentityRole<Guid>
                    { Id = Guid.NewGuid(), Name = "Organizer", NormalizedName = "ORGANIZER", ConcurrencyStamp = "3" }
            ]);
        });
    }
}