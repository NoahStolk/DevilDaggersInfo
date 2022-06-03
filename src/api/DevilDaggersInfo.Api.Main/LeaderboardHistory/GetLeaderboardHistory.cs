namespace DevilDaggersInfo.Api.Main.LeaderboardHistory;

public record GetLeaderboardHistory : IGetLeaderboardGlobalDto
{
	public DateTime DateTime { get; init; }

	public int TotalPlayers { get; init; }

	public double TimeGlobal { get; init; }

	public ulong KillsGlobal { get; init; }

	public ulong GemsGlobal { get; init; }

	public ulong DeathsGlobal { get; init; }

	public ulong DaggersHitGlobal { get; init; }

	public ulong DaggersFiredGlobal { get; init; }

	public List<GetEntryHistory> Entries { get; set; } = new();
}
