namespace TournamentBracket.Domain.Common.Abstractions;

public interface IEntity<TKey>
{
	public TKey Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}