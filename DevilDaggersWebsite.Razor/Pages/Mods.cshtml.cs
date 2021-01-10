﻿using DevilDaggersWebsite.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DevilDaggersWebsite.Razor.Pages
{
	public class ModsModel : PageModel
	{
		private readonly ApplicationDbContext _dbContext;

		public ModsModel(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public List<AssetMod> AssetMods { get; private set; } = new();

		public void OnGet()
		{
			Dictionary<AssetMod, string> sortedMods = new();
			foreach (AssetMod assetMod in _dbContext.AssetMods.Include(am => am.PlayerAssetMods).ThenInclude(pam => pam.Player))
			{
				string author = assetMod.PlayerAssetMods.Select(pam => pam.Player.Username).OrderBy(s => s).First();
				sortedMods.Add(assetMod, author);
			}

			AssetMods = sortedMods.OrderBy(kvp => kvp.Value).ThenBy(kvp => kvp.Key.Name).Select(kvp => kvp.Key).ToList();
		}
	}
}