namespace DevilDaggersInfo.Web.Server.InternalModels.CustomLeaderboards;

public class CustomEntryWithReplay
{
	public int Id { get; init; }

	public int PlayerId { get; init; }

	public string PlayerName { get; init; } = null!;

	public int Time { get; init; }

	public int GemsCollected { get; init; }

	public int EnemiesKilled { get; init; }

	public int DaggersFired { get; init; }

	public int DaggersHit { get; init; }

	public int EnemiesAlive { get; init; }

	public int HomingStored { get; init; }

	public int? HomingEaten { get; init; }

	public int? GemsDespawned { get; init; }

	public int? GemsEaten { get; init; }

	public int? GemsTotal { get; init; }

	public byte DeathType { get; init; }

	public int LevelUpTime2 { get; init; }

	public int LevelUpTime3 { get; init; }

	public int LevelUpTime4 { get; init; }

	public DateTime SubmitDate { get; init; }

	public string ClientVersion { get; init; } = null!;

	public bool HasReplay { get; init; }
}