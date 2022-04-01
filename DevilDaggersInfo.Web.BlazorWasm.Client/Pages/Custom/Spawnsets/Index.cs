using DevilDaggersInfo.Web.BlazorWasm.Client.Extensions;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Constants;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Spawnsets;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Enums.Sortings.Public;
using Microsoft.AspNetCore.Components;

namespace DevilDaggersInfo.Web.BlazorWasm.Client.Pages.Custom.Spawnsets;

public partial class Index : IHasNavigation
{
	private int _pageIndex;
	private int _pageSize = PagingConstants.PageSizeDefault;

	[Parameter, SupplyParameterFromQuery] public bool PracticeOnly { get; set; }
	[Parameter, SupplyParameterFromQuery] public bool WithCustomLeaderboardOnly { get; set; }
	[Parameter, SupplyParameterFromQuery] public string? SpawnsetFilter { get; set; }
	[Parameter, SupplyParameterFromQuery] public string? AuthorFilter { get; set; }
	[Parameter, SupplyParameterFromQuery] public int PageIndex { get => _pageIndex; set => _pageIndex = Math.Max(0, value); }
	[Parameter, SupplyParameterFromQuery] public int PageSize { get => _pageSize; set => _pageSize = value < PagingConstants.PageSizeMin || value > PagingConstants.PageSizeMax ? PagingConstants.PageSizeDefault : value; }
	[Parameter, SupplyParameterFromQuery] public int? SortBy { get; set; }
	[Parameter, SupplyParameterFromQuery] public bool Ascending { get; set; }

	private Dictionary<SpawnsetSorting, bool> _sortings = new();

	public Page<GetSpawnsetOverview>? GetSpawnsets { get; set; }

	public int TotalPages => GetSpawnsets == null ? 0 : (GetSpawnsets.TotalResults - 1) / PageSize + 1;
	public int TotalResults => GetSpawnsets == null ? 0 : GetSpawnsets.TotalResults;

	protected override async Task OnInitializedAsync()
	{
		foreach (SpawnsetSorting e in (SpawnsetSorting[])Enum.GetValues(typeof(SpawnsetSorting)))
			_sortings.Add(e, false);

		await Fetch();
	}

	private async Task ChangeInputSpawnsetName(ChangeEventArgs e)
	{
		SpawnsetFilter = e.Value?.ToString();
		NavigationManager.AddOrModifyQueryParameter(nameof(SpawnsetFilter), SpawnsetFilter);

		await Fetch();
	}

	private async Task ChangeInputAuthorName(ChangeEventArgs e)
	{
		AuthorFilter = e.Value?.ToString();
		NavigationManager.AddOrModifyQueryParameter(nameof(AuthorFilter), AuthorFilter);

		await Fetch();
	}

	private async Task ChangeInputPracticeOnly(ChangeEventArgs e)
	{
		PracticeOnly = bool.TryParse(e.Value?.ToString(), out bool value) && value;
		NavigationManager.AddOrModifyQueryParameter(nameof(PracticeOnly), PracticeOnly);

		await Fetch();
	}

	private async Task ChangeInputWithCustomLeaderboardOnly(ChangeEventArgs e)
	{
		WithCustomLeaderboardOnly = bool.TryParse(e.Value?.ToString(), out bool value) && value;
		NavigationManager.AddOrModifyQueryParameter(nameof(WithCustomLeaderboardOnly), WithCustomLeaderboardOnly);

		await Fetch();
	}

	public async Task ChangePageIndex(int pageIndex)
	{
		PageIndex = Math.Clamp(pageIndex, 0, TotalPages - 1);
		NavigationManager.AddOrModifyQueryParameter(nameof(PageIndex), PageIndex);

		await Fetch();

		StateHasChanged();
	}

	public async Task ChangePageSize(int pageSize)
	{
		PageSize = pageSize;
		NavigationManager.AddOrModifyQueryParameter(nameof(PageSize), PageSize);

		PageIndex = Math.Clamp(PageIndex, 0, TotalPages - 1);
		await Fetch();
	}

	private async Task Sort(SpawnsetSorting sortBy)
	{
		SortBy = (int)sortBy;
		_sortings[sortBy] = !_sortings[sortBy];
		Ascending = _sortings[sortBy];

		NavigationManager.AddOrModifyQueryParameters(new(nameof(SortBy), SortBy), new(nameof(Ascending), Ascending));

		await Fetch();
	}

	private async Task Fetch()
	{
		GetSpawnsets = await Http.GetSpawnsets(PracticeOnly, WithCustomLeaderboardOnly, SpawnsetFilter, AuthorFilter, PageIndex, PageSize, SortBy.HasValue ? (SpawnsetSorting)SortBy.Value : SpawnsetSorting.LastUpdated, Ascending);

		if (PageIndex >= TotalPages)
		{
			PageIndex = TotalPages - 1;
			NavigationManager.AddOrModifyQueryParameter(nameof(PageIndex), PageIndex);
		}
	}
}
