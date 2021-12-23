namespace DevilDaggersInfo.Web.BlazorWasm.Server.Caches.LeaderboardStatistics;

public class ArrayStatistics
{
	public ArrayStatistic Times { get; } = new();
	public ArrayStatistic Kills { get; } = new();
	public ArrayStatistic Gems { get; } = new();
	public ArrayStatistic DaggersFired { get; } = new();
	public ArrayStatistic DaggersHit { get; } = new();

	public void Populate(List<CompressedEntry> entries, int? limit = null)
	{
		if (limit.HasValue)
			entries = entries.Take(limit.Value).ToList();

		Times.Populate(entries.Select(e => e.Time.ToSecondsTime()));
		Kills.Populate(entries.Select(e => (double)e.Kills));
		Gems.Populate(entries.Select(e => (double)e.Gems));
		DaggersFired.Populate(entries.Select(e => (double)e.DaggersFired));
		DaggersHit.Populate(entries.Select(e => (double)e.DaggersHit));
	}
}