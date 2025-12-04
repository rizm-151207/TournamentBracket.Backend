using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentBracket.Api.Extensions;
using TournamentBracket.Application.Users;
using TournamentBracket.Application.Users.Commands;
using TournamentBracket.Application.Users.DTO;
using TournamentBracket.Application.Users.Interfaces;

namespace TournamentBracket.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] RegisterUserCommand registerUserCommand)
    {
        var signUpResult = await authService.Register(registerUserCommand);

        if (!signUpResult.IsSuccess)
            return BadRequest(signUpResult.Error);
        return Ok(new RegisterUserResponse(signUpResult.Item!.Email!));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand loginUserCommand)
    {
        var loginResult = await authService.Login(loginUserCommand);
        if (!loginResult.IsSuccess)
            return BadRequest(loginResult.Error);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            IsEssential = true
        };

        HttpContext.Response.Cookies.Append(AuthConstants.AccessTokenName, loginResult.Item!.AccessToken,
            cookieOptions);

        return Ok(new TokensResponse(
            loginResult.Item.RefreshToken));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userEmail = HttpContext.User.GetClaimByType(ClaimTypes.Email);
        if (userEmail == null)
            return Unauthorized();

        var result = await authService.Logout(userEmail);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        Response.Cookies.Append(AuthConstants.AccessTokenName, string.Empty, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            IsEssential = true,
            Expires = DateTime.Now.AddDays(-1)
        });

        return Ok();
    }

    [Authorize]
    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand refreshTokenCommand)
    {
        var userEmail = HttpContext.User.GetClaimByType(ClaimTypes.Email);
        if (userEmail == null)
            return Unauthorized();

        var newTokensResult = await authService.RefreshTokens(refreshTokenCommand, userEmail);

        if (!newTokensResult.IsSuccess)
            return BadRequest(newTokensResult.Error);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            IsEssential = true
        };

        HttpContext.Response.Cookies.Append(AuthConstants.AccessTokenName, newTokensResult.Item!.AccessToken,
            cookieOptions);

        return Ok(new TokensResponse(newTokensResult.Item.RefreshToken));
    }
}