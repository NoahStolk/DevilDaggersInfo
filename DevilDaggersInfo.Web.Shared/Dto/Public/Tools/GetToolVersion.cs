namespace DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Tools;

public record GetToolVersion
{
	public Version VersionNumber { get; init; } = null!;

	public DateTime Date { get; init; }

	public int DownloadCount { get; init; }

	public IReadOnlyList<GetToolVersionChange> Changes { get; init; } = new List<GetToolVersionChange>();
}
