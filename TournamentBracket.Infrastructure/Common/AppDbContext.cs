using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.SingleEliminationBracket;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Divisions;
using TournamentBracket.Domain.Matches;
using TournamentBracket.Domain.Users;

namespace TournamentBracket.Infrastructure.Common;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Competitor> Competitors { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<Competition> Competitions { get; set; }
    public DbSet<Division> Divisions { get; set; }
    public DbSet<SingleEliminationBracket> SingleEliminationBrackets { get; set; }
    public DbSet<BracketNode> BracketNodes { get; set; }
    public DbSet<Match> Matches { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.Load("TournamentBracket.Infrastructure"));
        AddDefaultRoles(builder);
    }

    private void AddDefaultRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole<Guid>>(entity =>
        {
            entity.HasData(
                new IdentityRole<Guid>
                {
                    Id = new Guid("c44c02b2-6f22-4c39-8115-dac5072285dd"),
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    ConcurrencyStamp = "1"
                },
                new IdentityRole<Guid>
                {
                    Id = new Guid("32018d01-edf4-4035-8921-c8cba937ef60"),
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    ConcurrencyStamp = "2"
                },
                new IdentityRole<Guid>
                {
                    Id = new Guid("10a1a059-bbea-42a3-a2e2-c7f4c248eb11"),
                    Name = "Organizer",
                    NormalizedName = "ORGANIZER",
                    ConcurrencyStamp = "3"
                }
            );
        });
    }
}