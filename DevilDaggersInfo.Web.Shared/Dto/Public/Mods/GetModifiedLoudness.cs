namespace DevilDaggersInfo.Web.Shared.Dto.Public.Mods;

public record GetModifiedLoudness
{
	public string AssetName { get; init; } = null!;

	public bool IsProhibited { get; init; }

	public float DefaultLoudness { get; init; }

	public float ModifiedLoudness { get; init; }
}
