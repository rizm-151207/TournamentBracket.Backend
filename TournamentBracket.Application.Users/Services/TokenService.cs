using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TournamentBracket.Application.Users.DTO;
using TournamentBracket.Application.Users.Interfaces;
using TournamentBracket.Domain.Users;
using Newtonsoft.Json;

namespace TournamentBracket.Application.Users.Services;

public class TokenService : ITokenService
{
    private readonly JwtTokenConfiguration tokenConfiguration;
    private readonly UserManager<User> userManager;

    public TokenService(
        IOptions<JwtTokenConfiguration> tokenConfiguration,
        UserManager<User> userManager)
    {
        this.tokenConfiguration = tokenConfiguration.Value;
        this.userManager = userManager;
    }

    public async ValueTask<string> GenerateAccessToken(User user, CancellationToken ct = default)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
            {
                new(ClaimTypes.Email, user.Email)
            }
            .Union(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var securityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfiguration.Secret));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(tokenConfiguration.ExpireInMinutes),
            issuer: tokenConfiguration.Issuer,
            audience: tokenConfiguration.Audience,
            signingCredentials: signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public ValueTask<string> GenerateRefreshToken(CancellationToken ct = default)
    {
        var randomBytes = new byte[64];

        var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomBytes);

        return ValueTask.FromResult(Convert.ToBase64String(randomBytes));
    }
}