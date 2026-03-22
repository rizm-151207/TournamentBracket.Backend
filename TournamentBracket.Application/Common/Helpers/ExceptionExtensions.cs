using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Common.Helpers;

public static class ExceptionExtensions
{
    public static Result ToResult(this Exception e, int code = 500)
    {
        var innerError = e.InnerException is not null ? new Error(e.InnerException.Message) : null;
        return Result.Failed(new Error(e.Message, code, innerError));
    }

    public static Result<T> ToResult<T>(this Exception e, int code = 500)
    {
        var innerError = e.InnerException is not null ? new Error(e.InnerException.Message) : null;
        return Result<T>.FailedWith(new Error(e.Message, code, innerError));
    }
}