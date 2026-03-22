namespace TournamentBracket.Infrastructure.Common.Results;

public class Error
{
    public string Message { get; private set; }
    public int? Code { get; private set; }
    public Error? InnerError { get; private set; }

    public Error(string message, int? code = null, Error? innerError = null)
    {
        Message = message;
        Code = code;
        InnerError = innerError;
    }
}