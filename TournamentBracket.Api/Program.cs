using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TournamentBracket.Api.Extensions;
using TournamentBracket.Application.Competitors.Interfaces;
using TournamentBracket.Application.Competitors.Services;
using TournamentBracket.Application.Users.DTO;
using TournamentBracket.Application.Users.Interfaces;
using TournamentBracket.Application.Users.Services;
using TournamentBracket.Infrastructure.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<JwtTokenConfiguration>(builder.Configuration.GetSection("Jwt"));

builder.AddSwaggerWithJwtAuth()
    .AddIdentityUser()
    .AddJwtBearerAuth()
    .AddTunedCors()
    .AddFluentValidation();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICompetitorService, CompetitorService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();