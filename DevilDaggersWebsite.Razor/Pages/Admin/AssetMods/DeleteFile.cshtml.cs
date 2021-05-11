﻿using DevilDaggersDiscordBot.Logging;
using DevilDaggersWebsite.Caches.ModArchive;
using DevilDaggersWebsite.Razor.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Razor.Pages.Admin.AssetMods
{
	public class DeleteFileModel : PageModel
	{
		private readonly IWebHostEnvironment _env;

		public DeleteFileModel(IWebHostEnvironment env)
		{
			_env = env;
		}

		public IEnumerable<string> ModFileNames { get; private set; } = Enumerable.Empty<string>();

		public void OnGet()
		{
			ModFileNames = Directory.GetFiles(Path.Combine(_env.WebRootPath, "mods")).Select(p => Path.GetFileName(p));
		}

		public async Task<ActionResult?> OnPost(string fileName)
		{
			string failedAttemptMessage = $":x: Failed attempt from `{this.GetIdentity()}` to delete ASSETMOD file";

			string path = Path.Combine(_env.WebRootPath, "mods", fileName);
			if (!System.IO.File.Exists(path))
			{
				await DiscordLogger.Instance.TryLog(Channel.AuditLogMonitoring, _env.EnvironmentName, $"{failedAttemptMessage}: File `{fileName}` does not exist.");
				return null;
			}

			System.IO.File.Delete(path);

			await DiscordLogger.Instance.TryLog(Channel.AuditLogMonitoring, _env.EnvironmentName, $":white_check_mark: `{this.GetIdentity()}` deleted ASSETMOD file :file_folder: `{fileName}`.");

			// Clear entire memory cache (can't clear individual entries).
			await ModArchiveCache.Instance.Clear(_env);

			// Clear file cache for this mod.
			string cacheFilePath = Path.Combine(_env.WebRootPath, "mod-archive-cache", $"{Path.GetFileNameWithoutExtension(fileName)}.json");
			if (System.IO.File.Exists(cacheFilePath))
			{
				System.IO.File.Delete(cacheFilePath);
				await DiscordLogger.Instance.TryLog(Channel.CacheMonitoring, _env.EnvironmentName, $"Deleted mod archive cache file `{Path.GetFileName(cacheFilePath)}`.");
			}

			return RedirectToPage("Index");
		}
	}
}
