namespace DevilDaggersInfo.Api.DdLive.LeaderboardStatistics;

public record GetArrayStatisticDdLive
{
	public double Average { get; init; }

	public double Median { get; init; }

	public double Mode { get; init; }
}
