﻿using DevilDaggersCore.CustomLeaderboards;
using DevilDaggersWebsite.Code.Database;
using DevilDaggersWebsite.Code.Database.CustomLeaderboards;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevilDaggersWebsite.Pages.CustomLeaderboards
{
	public class IndexModel : PageModel
	{
		public List<SelectListItem> CategoryListItems { get; private set; } = new List<SelectListItem>();

		public CustomLeaderboardCategory Category { get; private set; }
		public List<CustomLeaderboard> Leaderboards { get; private set; }

		private readonly ApplicationDbContext _context;

		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public void OnGet(string category)
		{
			foreach (CustomLeaderboardCategory clc in (CustomLeaderboardCategory[])Enum.GetValues(typeof(CustomLeaderboardCategory)))
				CategoryListItems.Add(new SelectListItem($"Category: {clc}", clc.ToString()));

			if (string.IsNullOrEmpty(category))
				Category = CustomLeaderboardCategory.Default;
			else
				Category = (CustomLeaderboardCategory)Enum.Parse(typeof(CustomLeaderboardCategory), category);

			Leaderboards = _context.CustomLeaderboards.Where(l => l.Category == Category).ToList();
		}
	}
}