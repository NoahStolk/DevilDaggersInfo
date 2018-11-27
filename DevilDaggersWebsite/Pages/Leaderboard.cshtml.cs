﻿using DevilDaggersWebsite.Models.Leaderboard;
using DevilDaggersWebsite.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Pages
{
	public class LeaderboardModel : PageModel
	{
		[BindProperty]
		public Leaderboard Leaderboard { get; set; } = new Leaderboard();

		public int Rank { get; set; }
		public string Search { get; set; }
		public bool IsUserSearch => !string.IsNullOrEmpty(Search) && Search.Length >= 3;

		public async Task OnGetAsync(int rank, string search)
		{
			Rank = Math.Max(rank, 1);
			Search = search;

			if (IsUserSearch)
			{
				Leaderboard = await LeaderboardUtils.LoadLeaderboardSearch(Search);
			}
			else
			{
				Leaderboard = await LeaderboardUtils.LoadLeaderboard(Rank);

				if (Rank > Leaderboard.Players - 99)
				{
					Rank = Leaderboard.Players - 99;
					Leaderboard.Entries.Clear();
					Leaderboard = await LeaderboardUtils.LoadLeaderboard(Rank);
				}
			}
		}
	}
}