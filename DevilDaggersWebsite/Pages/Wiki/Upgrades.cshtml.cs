﻿using DevilDaggersCore.Game;
using DevilDaggersWebsite.Code.PageModels;
using System.Collections.Generic;

namespace DevilDaggersWebsite.Pages.Wiki
{
	public class UpgradesModel : WikiPageModel
	{
		public List<Upgrade> upgrades;

		public void OnGet(string gameVersion)
		{
			SetGameVersion(gameVersion);

			upgrades = Game.GetEntities<Upgrade>(gameVersionObject);
		}
	}
}