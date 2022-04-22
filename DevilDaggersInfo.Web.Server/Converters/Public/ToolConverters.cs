using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Tools;
using DevilDaggersInfo.Web.Server.InternalModels;
using DevilDaggersInfo.Web.Server.InternalModels.Json;

namespace DevilDaggersInfo.Web.Server.Converters.Public;

public static class ToolConverters
{
	public static GetTool ToGetTool(this Tool tool, List<ToolStatisticEntity> toolStatistics, int fileSize) => new()
	{
		DisplayName = tool.DisplayName,
		FileSize = fileSize,
		Name = tool.Name,
		VersionNumber = tool.VersionNumber,
		VersionNumberRequired = tool.VersionNumberRequired,
		Changelog = tool.Changelog?
			.Select(ce => ce.ToGetToolVersion(toolStatistics.Find(ts => ts.VersionNumber == ce.VersionNumber.ToString())))
			.ToList(),
	};

	private static GetToolVersion ToGetToolVersion(this ChangelogEntry changelogEntry, ToolStatisticEntity? toolStatistic) => new()
	{
		Changes = changelogEntry.Changes
			.Select(c => c.ToGetToolVersionChange())
			.ToList(),
		DownloadCount = toolStatistic?.DownloadCount ?? 0,
		Date = changelogEntry.Date,
		VersionNumber = changelogEntry.VersionNumber,
	};

	private static GetToolVersionChange ToGetToolVersionChange(this Change change) => new()
	{
		Description = change.Description,
		SubChanges = change.SubChanges?
			.Select(c => c.ToGetToolVersionChange())
			.ToList(),
	};
}
