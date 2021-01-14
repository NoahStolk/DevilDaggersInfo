﻿using DevilDaggersWebsite.Clients;
using DevilDaggersWebsite.Dto;
using DevilDaggersWebsite.Razor.PageModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lb = DevilDaggersWebsite.Dto.Leaderboard;

namespace DevilDaggersWebsite.Razor.Pages.Leaderboard
{
	public class UserSettingsModel : PageModel, IDefaultLeaderboardPage
	{
		public int PageCount { get; } = 3;

		public Lb? Leaderboard { get; set; } = new Lb();

		public async Task OnGetAsync()
		{
			IEnumerable<Task> tasks = Enumerable.Range(0, PageCount).Select(async i =>
			{
				Lb? nextLeaderboard = await DdHasmodaiClient.GetScores(i * 100 + 1);
				foreach (Entry entry in nextLeaderboard?.Entries ?? new List<Entry>())
					Leaderboard!.Entries.Add(entry);
			});
			await Task.WhenAll(tasks);

			Leaderboard!.Entries = Leaderboard.Entries.OrderBy(e => e.Rank).ToList();
		}
	}
}
