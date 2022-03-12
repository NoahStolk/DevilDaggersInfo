using DevilDaggersInfo.Core.Spawnset.View;
using DevilDaggersInfo.Core.Spawnset;
using Microsoft.AspNetCore.Components;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Spawnsets;
using DevilDaggersInfo.Web.BlazorWasm.Client.HttpClients;

namespace DevilDaggersInfo.Web.BlazorWasm.Client.Pages.Custom.Spawnsets;

public partial class SpawnsetPage
{
	private bool _notFound;

	[Inject] public PublicApiHttpClient Http { get; set; } = null!;
	[Inject] public NavigationManager NavigationManager { get; set; } = null!;

	[Parameter, EditorRequired] public int Id { get; set; }

	public GetSpawnset? GetSpawnset { get; set; }

	public SpawnsetBinary? SpawnsetBinary { get; set; }
	public SpawnsView? SpawnsView { get; set; }
	public EffectivePlayerSettings EffectivePlayerSettings { get; set; }

	protected override async Task OnParametersSetAsync()
	{
		try
		{
			GetSpawnset = await Http.GetSpawnsetById(Id);
		}
		catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
		{
			_notFound = true;
			return;
		}

		if (!SpawnsetBinary.TryParse(GetSpawnset.FileBytes, out SpawnsetBinary? spawnsetBinary))
		{
			// TODO: Log error.
			return;
		}

		SpawnsetBinary = spawnsetBinary;
		SpawnsView = new(spawnsetBinary, GameConstants.CurrentVersion);
		EffectivePlayerSettings = spawnsetBinary.GetEffectivePlayerSettings();
	}
}
