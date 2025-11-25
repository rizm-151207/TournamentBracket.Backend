using Microsoft.AspNetCore.Identity;
using TournamentBracket.Domain.Share.Abstractions;

namespace TournamentBracket.Domain.Users;

public class User : IdentityUser<Guid>, IEntity<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}