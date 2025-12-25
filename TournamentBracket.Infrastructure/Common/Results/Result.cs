namespace TournamentBracket.Infrastructure.Common.Results;

public class Result
{
    public static Result Success() => new(true);
    public static Result Failed(Error error) => new(false, error);
    public static Result Failed(string errorMessage) => new(false, new Error(errorMessage));

    public bool IsSuccess { get; private set; }

    public Error? Error { get; private set; }

    protected Result(bool isSuccess, Error? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
}