namespace DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Admin.Mods;

public class GetModForOverview : IGetDto<int>
{
	public int Id { get; init; }

	public string Name { get; init; } = null!;

	public bool IsHidden { get; init; }

	public DateTime LastUpdated { get; init; }

	public string? TrailerUrl { get; init; }

	public string? HtmlDescription { get; init; }

	public ModTypes ModTypes { get; init; }

	public string? Url { get; init; }
}
