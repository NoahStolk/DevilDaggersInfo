﻿using DevilDaggersWebsite.Caches.ModArchive;
using DevilDaggersWebsite.HostedServices.DdInfoDiscordBot;
using DevilDaggersWebsite.Razor.PageModels;
using DevilDaggersWebsite.Singletons;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Razor.Pages.Admin.AssetMods
{
	public class DeleteFileModel : AbstractAdminPageModel
	{
		private readonly IWebHostEnvironment _environment;
		private readonly DiscordLogger _discordLogger;
		private readonly ModArchiveCache _modArchiveCache;

		public DeleteFileModel(IWebHostEnvironment environment, DiscordLogger discordLogger, ModArchiveCache modArchiveCache)
		{
			_environment = environment;
			_discordLogger = discordLogger;
			_modArchiveCache = modArchiveCache;
		}

		public IEnumerable<string> ModFileNames { get; private set; } = Enumerable.Empty<string>();

		public void OnGet()
		{
			ModFileNames = Directory.GetFiles(Path.Combine(_environment.WebRootPath, "mods")).Select(p => Path.GetFileName(p));
		}

		public async Task<ActionResult?> OnPost(string fileName)
		{
			string failedAttemptMessage = $":x: Failed attempt from `{GetIdentity()}` to delete ASSETMOD file";

			string path = Path.Combine(_environment.WebRootPath, "mods", fileName);
			if (!System.IO.File.Exists(path))
			{
				await _discordLogger.TryLog(Channel.MonitoringAuditLog, $"{failedAttemptMessage}: File `{fileName}` does not exist.");
				return null;
			}

			System.IO.File.Delete(path);

			await _discordLogger.TryLog(Channel.MonitoringAuditLog, $":white_check_mark: `{GetIdentity()}` deleted ASSETMOD file :file_folder: `{fileName}`.");

			// Clear entire memory cache (can't clear individual entries).
			_modArchiveCache.Clear();

			// Clear file cache for this mod.
			string cacheFilePath = Path.Combine(_environment.WebRootPath, "mod-archive-cache", $"{Path.GetFileNameWithoutExtension(fileName)}.json");
			if (System.IO.File.Exists(cacheFilePath))
				System.IO.File.Delete(cacheFilePath);

			return RedirectToPage("Index");
		}
	}
}