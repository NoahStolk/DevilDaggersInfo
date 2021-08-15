﻿namespace DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Admin.Spawnsets;

public class EditSpawnset
{
	[Required]
	public int PlayerId { get; set; }

	[StringLength(64)]
	public string Name { get; set; } = null!;

	[Range(0, 400)]
	public int? MaxDisplayWaves { get; set; }

	[StringLength(2048)]
	public string? HtmlDescription { get; set; }

	public bool IsPractice { get; set; }
}
