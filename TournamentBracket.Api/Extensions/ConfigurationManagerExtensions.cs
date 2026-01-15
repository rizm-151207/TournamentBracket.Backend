namespace TournamentBracket.Api.Extensions;

public static class ConfigurationManagerExtensions
{
    private const string DbUserVariableName = "TOURNAMENT_DB_USER";
    private const string DbPasswordVariableName = "TOURNAMENT_DB_PASSWORD";
    
    public static string? GetConnectionStringWithEnvironmentVariables(this IConfigurationManager manager, string key)
    {
        return manager.GetConnectionString(key)?
            .Replace($"${{{DbUserVariableName}}}", Environment.GetEnvironmentVariable(DbUserVariableName))
            .Replace($"${{{DbPasswordVariableName}}}", Environment.GetEnvironmentVariable(DbPasswordVariableName));
    }
}