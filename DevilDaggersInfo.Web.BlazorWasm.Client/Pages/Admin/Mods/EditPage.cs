using DevilDaggersInfo.Core.Extensions;
using DevilDaggersInfo.Web.BlazorWasm.Client.Components.Admin;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Constants;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Admin.Mods;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Admin.Players;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace DevilDaggersInfo.Web.BlazorWasm.Client.Pages.Admin.Mods;

public partial class EditPage
{
	private Dictionary<int, string>? _playerNames;
	private EditMod _editMod = new();
	private AdminEdit<EditMod> _editComponent = null!;
	private List<string> _binaryFileSizeErrors = new();
	private List<string> _screenshotFileSizeErrors = new();

	[Parameter, EditorRequired] public int Id { get; set; }

	private IReadOnlyList<string>? _binaryNames;
	private IReadOnlyList<string>? _screenshotNames;

	protected override async Task OnInitializedAsync()
	{
		try
		{
			List<GetPlayerName> getPlayerNames = await Http.GetPlayerNames();
			_playerNames = getPlayerNames.ToDictionary(m => m.Id, m => m.PlayerName);

			GetMod getMod = await Http.GetModById(Id);
			_editMod = new()
			{
				ModTypes = getMod.ModTypes.AsEnumerable().ToList(),
				HtmlDescription = getMod.HtmlDescription,
				IsHidden = getMod.IsHidden,
				Name = getMod.Name,
				PlayerIds = getMod.PlayerIds,
				TrailerUrl = getMod.TrailerUrl,
				Url = getMod.Url,
			};

			_binaryNames = getMod.BinaryNames;
			_screenshotNames = getMod.ScreenshotNames;

			_editComponent.State = ErrorState.None;
		}
		catch (HttpRequestException ex)
		{
			if (ex.StatusCode.HasValue)
				_editComponent.ErrorMessage = $"Error {(int)ex.StatusCode}: {ex.StatusCode}";
			else
				_editComponent.ErrorMessage = "An error occurred while sending the request.";

			_editComponent.State = ErrorState.FatalError;
		}
		catch (AccessTokenNotAvailableException exception)
		{
			exception.Redirect();
		}
	}

	private async Task LoadBinaries(InputFileChangeEventArgs e)
	{
		_editMod.Binaries = await GetFiles(e, ModConstants.BinaryMaxFiles, ModConstants.BinaryMaxFileSize, _binaryFileSizeErrors);
	}

	private async Task LoadScreenshots(InputFileChangeEventArgs e)
	{
		_editMod.Screenshots = await GetFiles(e, ModConstants.ScreenshotMaxFiles, ModConstants.ScreenshotMaxFileSize, _screenshotFileSizeErrors);
	}
}
