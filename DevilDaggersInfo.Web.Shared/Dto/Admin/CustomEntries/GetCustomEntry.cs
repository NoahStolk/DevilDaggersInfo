namespace DevilDaggersInfo.Web.Shared.Dto.Admin.CustomEntries;

public record GetCustomEntry
{
	public int Id { get; init; }

	public int SpawnsetId { get; init; }

	public int PlayerId { get; init; }

	public double Time { get; init; }

	public int GemsCollected { get; init; }

	public int GemsDespawned { get; init; }

	public int GemsEaten { get; init; }

	public int GemsTotal { get; init; }

	public int EnemiesKilled { get; init; }

	public int EnemiesAlive { get; init; }

	public int DaggersFired { get; init; }

	public int DaggersHit { get; init; }

	public int HomingStored { get; init; }

	public int HomingEaten { get; init; }

	public CustomEntryDeathType DeathType { get; init; }

	public double LevelUpTime2 { get; init; }

	public double LevelUpTime3 { get; init; }

	public double LevelUpTime4 { get; init; }

	public DateTime SubmitDate { get; init; }

	public string ClientVersion { get; init; } = null!;
}
