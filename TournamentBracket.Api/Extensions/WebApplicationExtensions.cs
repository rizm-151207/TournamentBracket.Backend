namespace TournamentBracket.Api.Extensions;

public static class WebApplicationExtensions
{
	public static WebApplication UseTunedCors(this WebApplication app)
	{
		app.UseCors("cors");
		return app;
	}
}