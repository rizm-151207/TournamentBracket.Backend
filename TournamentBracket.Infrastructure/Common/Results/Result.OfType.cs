namespace TournamentBracket.Infrastructure.Common.Results;

public class Result<TItem> : Result
{
	public static Result<TItem> Success(TItem item) => new(true, item);
	public static Result<TItem> FailedWith(Error error) => new(false, error: error);
	public static Result<TItem> FailedWith(string errorMessage) => new(false, error: new Error(errorMessage));

	public TItem? Item { get; private set; }

	protected Result(bool isSuccess, TItem? item = default, Error? error = null) : base(isSuccess, error)
	{
		Item = item;
	}
}