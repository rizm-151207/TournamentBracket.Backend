using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TournamentBracket.Api.Extensions;
using TournamentBracket.Application.Brackets.Interfaces;
using TournamentBracket.Application.Brackets.Services;
using TournamentBracket.Application.Common.Authorization;
using TournamentBracket.Application.Common.Authorization.Interfaces;
using TournamentBracket.Application.Competitions.Authorization;
using TournamentBracket.Application.Competitions.Interfaces;
using TournamentBracket.Application.Competitions.Services;
using TournamentBracket.Application.Competitors.Interfaces;
using TournamentBracket.Application.Competitors.Services;
using TournamentBracket.Application.Divisions.Interfaces;
using TournamentBracket.Application.Divisions.Services;
using TournamentBracket.Application.Matches.Interface;
using TournamentBracket.Application.Matches.Services;
using TournamentBracket.Application.Users.DTO;
using TournamentBracket.Application.Users.Interfaces;
using TournamentBracket.Application.Users.Services;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.RoundRobinBracket;
using TournamentBracket.Domain.Brackets.SingleEliminationBracket;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Domain.Divisions;
using TournamentBracket.Domain.Matches;
using TournamentBracket.Infrastructure.Brackets;
using TournamentBracket.Infrastructure.Brackets.Interfaces;
using TournamentBracket.Infrastructure.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseNpgsql(builder.Configuration.GetConnectionStringWithEnvironmentVariables("DefaultConnection"),
			b => b.MigrationsAssembly("TournamentBracket.Api"))
		.ConfigureWarnings(b =>
			b.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));
});

builder.Services.Configure<JwtTokenConfiguration>(builder.Configuration.GetSection("Jwt"));

builder.AddSwaggerWithJwtAuth()
	.AddIdentityUser()
	.AddJwtBearerAuth()
	.AddTunedCors()
	.AddFluentValidation();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IResourceAuthorizationService, ResourceAuthorizationService>();
builder.Services.AddSingleton<IAuthorizationHandler, CompetitionAuthorizationHandler>();
builder.Services.AddScoped<ICompetitorService, CompetitorService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<ICompetitionsService, CompetitionsService>();
builder.Services.AddScoped<IDivisionsService, DivisionsService>();
builder.Services.AddScoped<DivisionsFactory>();
builder.Services.AddScoped<ITournamentBracketsService, TournamentBracketsService>();
builder.Services.AddScoped<SingleEliminationBracketFactory>();
builder.Services.AddScoped<RoundRobinBracketFactory>();
builder.Services.AddScoped<MatchFactory>();
builder.Services.AddScoped<BracketNodeFactory>();
builder.Services.AddScoped<BracketTypeResolver>();
builder.Services.AddScoped<BracketFactoryResolver>();
builder.Services.AddScoped<IBracketsRepository, BracketsRepository>();
builder.Services.AddScoped<IMatchesService, MatchesService>();
builder.Services.AddScoped<ICompetitionPlanner, CompetitionPlanner>();
builder.Services.AddScoped<TatamiDistributor>();
builder.Services.AddScoped<MatchPlanner>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseTunedCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (args.Contains("--migrate"))
{
	using var scope = app.Services.CreateScope();
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	dbContext.Database.Migrate();
}

app.Run();