namespace TournamentBracket.Domain.Share.Abstractions;

public interface IEntity<TKey>
{
    public TKey Id { get; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}