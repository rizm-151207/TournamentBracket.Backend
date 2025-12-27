using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Users;

namespace TournamentBracket.Infrastructure.Common;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Competitor> Competitors { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<Competition> Competitions { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetCallingAssembly());
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