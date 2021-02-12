﻿using DevilDaggersWebsite.Enumerators;
using System;

namespace DevilDaggersWebsite.Dto
{
	public class CustomLeaderboard
	{
		public string SpawnsetName { get; set; } = null!;
		public string SpawnsetAuthorName { get; set; } = null!;
		public int Bronze { get; set; }
		public int Silver { get; set; }
		public int Golden { get; set; }
		public int Devil { get; set; }
		public int Homing { get; set; }
		public DateTime? DateLastPlayed { get; set; }
		public DateTime? DateCreated { get; set; }
		public CustomLeaderboardCategory Category { get; set; }
		public bool IsAscending { get; set; }
	}
}
