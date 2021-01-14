﻿using DevilDaggersCore.Game;
using DevilDaggersWebsite.Razor.PageModels;
using System.Collections.Generic;

namespace DevilDaggersWebsite.Razor.Pages.Wiki
{
	public class UpgradesModel : WikiPageModel
	{
		public List<Upgrade> Upgrades { get; private set; } = null!;

		public void OnGet(GameVersion gameVersion = GameVersion.V3)
		{
			SetGameVersion(gameVersion);

			Upgrades = GameInfo.GetEntities<Upgrade>(GameVersion);
		}
	}
}
