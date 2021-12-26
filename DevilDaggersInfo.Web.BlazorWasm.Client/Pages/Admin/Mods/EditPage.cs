using DevilDaggersInfo.Web.BlazorWasm.Client.Components.Admin;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Admin.Mods;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Admin.Players;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Enums;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components;
using DevilDaggersInfo.Core.Extensions;

namespace DevilDaggersInfo.Web.BlazorWasm.Client.Pages.Admin.Mods;

public partial class EditPage
{
	private Dictionary<int, string>? _playerNames;
	private EditMod? _editMod;
	private AdminEdit<EditMod> _editComponent = null!;
	private Dictionary<int, string> _modTypes = Enum.GetValues<ModTypes>().ToDictionary(e => (int)e, e => e.ToString());

	[Parameter, EditorRequired] public int Id { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

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

	//private async Task LoadFile(InputFileChangeEventArgs e)
	//{
	//	IBrowserFile file = e.File;
	//	using MemoryStream ms = new();
	//	await file.OpenReadStream().CopyToAsync(ms);

	//	_editMod!.FileContents = ms.ToArray();
	//}
}
