namespace DevilDaggersInfo.Api.Admin.Spawnsets;

public record GetSpawnset
{
	public int Id { get; init; }

	public int PlayerId { get; init; }

	public string Name { get; init; } = null!;

	public int? MaxDisplayWaves { get; init; }

	public string? HtmlDescription { get; init; }

	public DateTime LastUpdated { get; init; }

	public bool IsPractice { get; init; }
}
