﻿using DevilDaggersWebsite.BlazorWasm.Shared.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevilDaggersWebsite.BlazorWasm.Shared.Dto.Admin.Mods
{
	public class AddMod
	{
		[StringLength(64)]
		public string Name { get; set; } = null!;

		public bool IsHidden { get; set; }

		[StringLength(64)]
		public string? TrailerUrl { get; set; }

		[StringLength(2048)]
		public string? HtmlDescription { get; set; }

		public List<int>? AssetModTypes { get; set; }

		[StringLength(128)]
		public string? Url { get; set; }

		public List<int>? PlayerIds { get; set; }

		[MaxLength(ModFileConstants.MaxFileSize, ErrorMessage = ModFileConstants.MaxFileSizeErrorMessage)]
		public byte[]? FileContents { get; set; }
	}
}
