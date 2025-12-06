using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TournamentBracket.Application.Users.Commands;
using TournamentBracket.Application.Users.DTO;
using TournamentBracket.Application.Users.Interfaces;
using TournamentBracket.Domain.Users;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Users.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> userManager;
    private readonly ITokenService tokenService;
    private readonly JwtTokenConfiguration jwtTokenConfiguration;

    public AuthService(
        UserManager<User> userManager,
        ITokenService tokenService,
        IOptions<JwtTokenConfiguration> jwtTokenConfiguration)
    {
        this.userManager = userManager;
        this.tokenService = tokenService;
        this.jwtTokenConfiguration = jwtTokenConfiguration.Value;
    }

    public async Task<Result<User>> Register(RegisterUserCommand command, CancellationToken ct = default)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = command.Email,
            Email = command.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var result = await userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
            return Result<User>.FailedWith(string.Join(',', result.Errors));

        result = await userManager.AddToRoleAsync(user, command.Role);
        return result.Succeeded ? Result<User>.Success(user) : Result<User>.FailedWith(string.Join(',', result.Errors));
    }

    public async Task<Result<TokensPair>> Login(LoginUserCommand command, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user == null)
            return Result<TokensPair>.FailedWith($"Can't find user by email: {command.Email}");

        var isSuccess = await userManager.CheckPasswordAsync(user, command.Password);
        if (!isSuccess)
            return Result<TokensPair>.FailedWith("Wrong credentials");

        var accessToken = await tokenService.GenerateAccessToken(user, ct);
        var refreshToken = await tokenService.GenerateRefreshToken(ct);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime =
            DateTime.UtcNow + TimeSpan.FromMinutes(jwtTokenConfiguration.ExpireRefreshTokenInMinutes);

        return Result<TokensPair>.Success(new TokensPair(accessToken, refreshToken));
    }

    public async Task<Result> Logout(string userEmail, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(userEmail);

        if (user is null)
            return Result.Failed($"Can't find user by email {userEmail}");

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded ? Result.Success() : Result.Failed(string.Join(',', result.Errors));
    }

    public async Task<Result<TokensPair>> RefreshTokens(RefreshTokenCommand command, string userEmail,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return Result<TokensPair>.FailedWith($"Can't find user by email: {userEmail}");

        if (user.RefreshToken != command.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            return Result<TokensPair>.FailedWith("Refresh token expired");

        var accessToken = await tokenService.GenerateAccessToken(user, ct);
        var refreshToken = await tokenService.GenerateRefreshToken(ct);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime =
            DateTime.UtcNow + TimeSpan.FromMinutes(jwtTokenConfiguration.ExpireRefreshTokenInMinutes);

        return Result<TokensPair>.Success(new TokensPair(accessToken, refreshToken));
    }
}