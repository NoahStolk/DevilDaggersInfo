namespace DevilDaggersInfo.Web.Shared.Dto.Public.Mods;

public record GetModDdae
{
	public string Name { get; init; } = null!;

	public string? HtmlDescription { get; init; }

	public string? TrailerUrl { get; init; }

	public List<string> Authors { get; init; } = null!;

	public DateTime LastUpdated { get; init; }

	public ModTypes AssetModTypes { get; init; }

	public bool IsHostedOnDdInfo { get; init; }

	public bool? ContainsProhibitedAssets { get; init; }

	public GetModArchiveDdae? ModArchive { get; init; }

	public List<string> ScreenshotFileNames { get; init; } = null!;
}