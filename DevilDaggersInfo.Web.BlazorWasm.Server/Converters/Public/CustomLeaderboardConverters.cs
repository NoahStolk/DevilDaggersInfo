using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.CustomLeaderboards;

namespace DevilDaggersInfo.Web.BlazorWasm.Server.Converters.Public;

public static class CustomLeaderboardConverters
{
	public static GetCustomLeaderboardDdcl ToGetCustomLeaderboardDdcl(this CustomLeaderboardEntity customLeaderboard) => new()
	{
		SpawnsetAuthorName = customLeaderboard.Spawnset.Player.PlayerName,
		SpawnsetName = customLeaderboard.Spawnset.Name,
		TimeBronze = customLeaderboard.TimeBronze,
		TimeSilver = customLeaderboard.TimeSilver,
		TimeGolden = customLeaderboard.TimeGolden,
		TimeDevil = customLeaderboard.TimeDevil,
		TimeLeviathan = customLeaderboard.TimeLeviathan,
		DateLastPlayed = customLeaderboard.DateLastPlayed,
		DateCreated = customLeaderboard.DateCreated,
		Category = customLeaderboard.Category,
		IsAscending = customLeaderboard.Category.IsAscending(),
	};

	public static GetCustomLeaderboardOverview ToGetCustomLeaderboardOverview(this CustomLeaderboardEntity customLeaderboard, string? topPlayer, int? worldRecord) => new()
	{
		Id = customLeaderboard.Id,
		SpawnsetAuthorName = customLeaderboard.Spawnset.Player.PlayerName,
		SpawnsetName = customLeaderboard.Spawnset.Name,
		TimeBronze = customLeaderboard.TimeBronze.ToSecondsTime(),
		TimeSilver = customLeaderboard.TimeSilver.ToSecondsTime(),
		TimeGolden = customLeaderboard.TimeGolden.ToSecondsTime(),
		TimeDevil = customLeaderboard.TimeDevil.ToSecondsTime(),
		TimeLeviathan = customLeaderboard.TimeLeviathan.ToSecondsTime(),
		DateCreated = customLeaderboard.DateCreated,
		DateLastPlayed = customLeaderboard.DateLastPlayed,
		TopPlayer = topPlayer,
		WorldRecord = worldRecord.ToSecondsTime(),
		WorldRecordDagger = worldRecord.HasValue ? customLeaderboard.GetDaggerFromTime(worldRecord.Value) : CustomLeaderboardDagger.Default,
	};

	public static GetCustomLeaderboard ToGetCustomLeaderboard(this CustomLeaderboardEntity customLeaderboard) => new()
	{
		SpawnsetAuthorName = customLeaderboard.Spawnset.Player.PlayerName,
		SpawnsetName = customLeaderboard.Spawnset.Name,
		TimeBronze = customLeaderboard.TimeBronze.ToSecondsTime(),
		TimeSilver = customLeaderboard.TimeSilver.ToSecondsTime(),
		TimeGolden = customLeaderboard.TimeGolden.ToSecondsTime(),
		TimeDevil = customLeaderboard.TimeDevil.ToSecondsTime(),
		TimeLeviathan = customLeaderboard.TimeLeviathan.ToSecondsTime(),
		DateCreated = customLeaderboard.DateCreated,
		TotalRunsSubmitted = customLeaderboard.DateCreated < FeatureConstants.TrackingCustomLeaderboardSubmitCounts ? null : customLeaderboard.TotalRunsSubmitted,
		Category = customLeaderboard.Category,
		IsArchived = customLeaderboard.IsArchived,
		DateLastPlayed = customLeaderboard.DateLastPlayed,
		CustomEntries = customLeaderboard.CustomEntries?.OrderBy(ce => ce.Time, customLeaderboard.Category.IsAscending()).Select((ce, i) => ce.ToGetCustomEntry(i + 1)).ToList() ?? new(),
	};
}