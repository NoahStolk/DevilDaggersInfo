﻿using DevilDaggersWebsite.Caches.ModArchive;
using DevilDaggersWebsite.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Io = System.IO;

namespace DevilDaggersWebsite.Razor.Pages
{
	public class ModModel : PageModel
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly IWebHostEnvironment _environment;
		private readonly ModArchiveCache _modArchiveCache;

		public ModModel(ApplicationDbContext dbContext, IWebHostEnvironment environment, ModArchiveCache modArchiveCache)
		{
			_dbContext = dbContext;
			_environment = environment;
			_modArchiveCache = modArchiveCache;
		}

		public string? Query { get; }
		public AssetMod? AssetMod { get; private set; }

		public bool IsHostedOnDdInfo { get; private set; }

		public ModArchiveCacheData ArchiveData { get; private set; } = new();
		public bool ContainsProhibitedAssets { get; private set; }

		public List<string> Images { get; private set; } = new();

		public ActionResult? OnGet()
		{
			AssetMod = _dbContext.AssetMods
				.AsNoTracking()
				.Include(am => am.PlayerAssetMods)
					.ThenInclude(pam => pam.Player)
				.FirstOrDefault(am => am.Name == HttpContext.Request.Query["mod"].ToString() && !am.IsHidden);
			if (AssetMod == null)
				return RedirectToPage("Mods");

			string screenshotsPath = Path.Combine(_environment.WebRootPath, "mod-screenshots", AssetMod.Name);
			if (Directory.Exists(screenshotsPath))
				Images = Directory.GetFiles(screenshotsPath).Select(p => Path.GetFileName(p)).ToList();

			string zipPath = Path.Combine(_environment.WebRootPath, "mods", $"{AssetMod.Name}.zip");
			IsHostedOnDdInfo = Io.File.Exists(zipPath);
			if (IsHostedOnDdInfo)
			{
				ArchiveData = _modArchiveCache.GetArchiveDataByFilePath(zipPath);
				ContainsProhibitedAssets = ArchiveData.Binaries.Any(md => md.Chunks.Any(mad => mad.IsProhibited));
			}

			return null;
		}
	}
}