﻿using DevilDaggersDiscordBot.Logging;
using DevilDaggersWebsite.Razor.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Razor.Pages.Admin.AssetMods
{
	public class DeleteScreenshotModel : PageModel
	{
		private readonly IWebHostEnvironment _env;

		public DeleteScreenshotModel(IWebHostEnvironment env)
		{
			_env = env;
		}

		public List<string> ModFileNames { get; } = new();

		public void OnGet()
		{
			foreach (string path in Directory.GetFiles(Path.Combine(_env.WebRootPath, "mod-screenshots"), "*.png", SearchOption.AllDirectories))
			{
				string directoryName = new DirectoryInfo(path).Parent?.Name ?? throw new($"Invalid path '{path}' while scanning mod screenshot file sizes.");
				ModFileNames.Add(Path.Combine(directoryName, Path.GetFileName(path)));
			}
		}

		public async Task<ActionResult?> OnPost(string fileName)
		{
			string failedAttemptMessage = $":x: Failed attempt from `{this.GetIdentity()}` to delete ASSETMOD screenshot";

			string path = Path.Combine(_env.WebRootPath, "mod-screenshots", fileName);
			if (!System.IO.File.Exists(path))
			{
				await DiscordLogger.Instance.TryLog(Channel.AuditLogMonitoring, _env.EnvironmentName, $"{failedAttemptMessage}: File '{fileName}' does not exist.");
				return null;
			}

			System.IO.File.Delete(path);

			await DiscordLogger.Instance.TryLog(Channel.AuditLogMonitoring, _env.EnvironmentName, $":white_check_mark: `{this.GetIdentity()}` deleted ASSETMOD screenshot `{fileName}`");

			return RedirectToPage("Index");
		}
	}
}