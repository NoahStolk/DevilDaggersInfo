namespace DevilDaggersInfo.Web.Shared.Dto.Public.CustomLeaderboards;

public record GetGlobalCustomLeaderboard
{
	public List<GetGlobalCustomLeaderboardEntry> Entries { get; init; } = new();

	public int TotalLeaderboards { get; init; }

	public int TotalPoints { get; init; }
}
