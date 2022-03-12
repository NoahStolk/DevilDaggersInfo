namespace DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.CustomLeaderboards;

public class GetCustomLeaderboardDdcl
{
	public string SpawnsetName { get; init; } = null!;

	public int TimeBronze { get; init; }

	public int TimeSilver { get; init; }

	public int TimeGolden { get; init; }

	public int TimeDevil { get; init; }

	public int TimeLeviathan { get; init; }

	public CustomLeaderboardCategory Category { get; init; }

	public bool IsAscending { get; init; }
}
