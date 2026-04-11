using TournamentBracket.Domain.Common.Abstractions;
using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Domain.Matches;

public class Match : IEntity<Guid>
{
	public Guid Id { get; set; }
	public string? Index { get; set; }
	public Competitor? FirstCompetitor { get; set; }
	public Competitor? SecondCompetitor { get; set; }
	public MatchStatus Status { get; set; }
	public required MatchProcess MatchProcess { get; set; }
	public DateTime? PlannedDateTime { get; set; }
	public DateTime? StartDateTime { get; set; }
	public DateTime? EndDateTime { get; set; }

	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }

	public bool IsByeMatch => Status is MatchStatus.Finished && MatchProcess.WinReason == WinReason.Bye;

	public void AddCompetitor(Competitor competitor)
	{
		if (FirstCompetitor is null)
		{
			FirstCompetitor = competitor;
			Status = SecondCompetitor is null
				? MatchStatus.WaitingOtherCompetitor
				: MatchStatus.Unplanned;
			return;
		}

		if (SecondCompetitor is null)
		{
			SecondCompetitor = competitor;
			Status = FirstCompetitor is null
				? MatchStatus.WaitingOtherCompetitor
				: MatchStatus.Unplanned;
			return;
		}

		throw new InvalidOperationException($"Can't add competitor to match. Match {Id} already full");
	}

	public void RemoveCompetitor(Competitor competitor)
	{
		if (FirstCompetitor != competitor && SecondCompetitor != competitor)
			throw new InvalidOperationException("Can't find competitor in competition to remove");

		if (FirstCompetitor == competitor)
			FirstCompetitor = null;
		if (SecondCompetitor == competitor)
			SecondCompetitor = null;

		MatchProcess.Clear();
		if (FirstCompetitor != null || SecondCompetitor != null)
		{
			MatchProcess.SetWinner(FirstCompetitor is not null, WinReason.Bye);
			Status = MatchStatus.Finished;
			CommitMatchFinish();
		}

	}

	public bool TryGetWinner(out Competitor? winner)
	{
		if (Status is MatchStatus.Finished)
		{
			if (MatchProcess.Winner == MatchWinner.FirstCompetitor)
			{
				winner = FirstCompetitor;
				return true;
			}

			if (MatchProcess.Winner == MatchWinner.SecondCompetitor)
			{
				winner = SecondCompetitor;
				return true;
			}
		}

		winner = null;
		return false;
	}

	public bool TryGetLoser(out Competitor? loser)
	{
		if (Status is MatchStatus.Finished)
		{
			if (MatchProcess.Winner == MatchWinner.FirstCompetitor && SecondCompetitor is not null)
			{
				loser = SecondCompetitor;
				return true;
			}

			if (MatchProcess.Winner == MatchWinner.SecondCompetitor && SecondCompetitor is not null)
			{
				loser = FirstCompetitor;
				return true;
			}
		}

		loser = null;
		return false;
	}

	public void Plan(int tatamiNum, int index, DateTime plannedTime)
	{
		Index = $"Поединок {index} (татами {tatamiNum})";
		PlannedDateTime = plannedTime;
		Status = MatchStatus.Planned;
	}

	public void UnplanBye()
	{
		if (!IsByeMatch)
			throw new InvalidOperationException($"{nameof(UnplanBye)} method can be used only with bye match");
		Index = null;
		PlannedDateTime = null;
	}


	public void UpdateMatch(MatchUpdateEvent updateEvent)
	{
		switch (updateEvent.Type)
		{
			case MatchUpdateType.Keikoku:
				MatchProcess.AddKeikoku(updateEvent.IsFirstCompetitor);
				break;
			case MatchUpdateType.Chui:
				MatchProcess.AddChui(updateEvent.IsFirstCompetitor);
				break;
			case MatchUpdateType.Genten:
				HandleGenten(updateEvent.IsFirstCompetitor);
				break;
			case MatchUpdateType.Wazari:
				MatchProcess.AddWazari(updateEvent.IsFirstCompetitor);
				break;
			case MatchUpdateType.Ippon:
				HandleIppon(updateEvent.IsFirstCompetitor);
				break;
			default: throw new ArgumentOutOfRangeException(nameof(updateEvent.Type));
		}

		TryFinishMatch();
	}

	private void HandleIppon(bool isFirstCompetitor)
	{
		MatchProcess.SetWinner(isFirstCompetitor, WinReason.Ippon);
		CommitMatchFinish();
	}

	private void HandleGenten(bool isFirstCompetitor)
	{
		MatchProcess.SetWinner(!isFirstCompetitor, WinReason.Sikkaku);
		CommitMatchFinish();
	}

	private void TryFinishMatch()
	{
		if (MatchProcess.FirstCompetitorWazari == 2 ^ MatchProcess.SecondCompetitorWazari == 2)
		{
			MatchProcess.SetWinner(MatchProcess.FirstCompetitorWazari == 2, WinReason.Wazari);
			CommitMatchFinish();
		}

		if (MatchProcess.FirstCompetitorChui == 2 ^ MatchProcess.SecondCompetitorChui == 2)
		{
			MatchProcess.SetWinner(MatchProcess.SecondCompetitorChui == 2, WinReason.Sikkaku);
			CommitMatchFinish();
		}
	}

	private void CommitMatchFinish()
	{
		Status = MatchStatus.Finished;
		EndDateTime = DateTime.UtcNow;
	}

	public void Clear()
	{
		FirstCompetitor = null;
		SecondCompetitor = null;
		PlannedDateTime = null;
		StartDateTime = null;
		EndDateTime = null;
		MatchProcess.WinReason = null;
		MatchProcess.Winner = null;
	}
}