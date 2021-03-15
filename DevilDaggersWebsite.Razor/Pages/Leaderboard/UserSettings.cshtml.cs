﻿using DevilDaggersWebsite.Clients;
using DevilDaggersWebsite.Dto;
using DevilDaggersWebsite.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Razor.Pages.Leaderboard
{
	public class UserSettingsModel : PageModel
	{
		private readonly ApplicationDbContext _dbContext;

		public UserSettingsModel(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public List<Entry>? Entries { get; private set; }

		public List<Player> Players { get; private set; } = new();

		public async Task OnGetAsync()
		{
			Players = _dbContext.Players.Include(p => p.PlayerTitles).ThenInclude(pt => pt.Title).Where(p => p.Dpi != null).ToList();

			Entries = await DdHasmodaiClient.GetUsersByIds(Players.Select(p => p.Id));
			if (Entries != null)
				Entries = Entries.OrderBy(e => e.Rank).ToList();
		}
	}
}
