﻿using DevilDaggersInfo.Web.BlazorWasm.Shared.Enums;
using System;
using System.Collections.Generic;

namespace DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Mods
{
	public class GetMod
	{
		public string Name { get; init; } = null!;

		public string? HtmlDescription { get; init; }

		public string? TrailerUrl { get; init; }

		public List<string> Authors { get; init; } = null!;

		public DateTime LastUpdated { get; init; }

		public AssetModTypes AssetModTypes { get; init; }

		public bool IsHostedOnDdInfo { get; init; }

		public bool? ContainsProhibitedAssets { get; init; }

		public GetModArchive? ModArchive { get; init; }

		public List<string> ScreenshotFileNames { get; init; } = null!;
	}
}
