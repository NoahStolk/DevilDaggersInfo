﻿namespace DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Tools;

public class GetChangelogEntry
{
	public Version VersionNumber { get; init; } = null!;

	public DateTime Date { get; init; }

	public IReadOnlyList<GetChange> Changes { get; init; } = new List<GetChange>();
}
