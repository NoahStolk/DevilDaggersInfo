namespace DevilDaggersInfo.Api.Ddae.Assets;

public record GetAssetInfo
{
	public string Name { get; init; } = null!;

	public string Description { get; init; } = null!;

	public List<string> Tags { get; init; } = null!;
}
