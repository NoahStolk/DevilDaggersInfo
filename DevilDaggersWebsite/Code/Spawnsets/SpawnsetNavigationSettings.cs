﻿using DevilDaggersWebsite.Pages;

namespace DevilDaggersWebsite.Code.Spawnsets
{
	public class SpawnsetNavigationSettings
	{
		public SpawnsetNavigationSettings(SpawnsetsModel model, int maxAround, ScreenWidthVisibility visibility)
		{
			Model = model;
			MaxAround = maxAround;
			Visibility = visibility;
		}

		public SpawnsetsModel Model { get; set; }
		public int MaxAround { get; set; }
		public ScreenWidthVisibility Visibility { get; set; }
	}
}