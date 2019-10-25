﻿using DevilDaggersWebsite.Code.Tasks.Scheduling;
using DevilDaggersWebsite.Pages.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Code.Tasks
{
	public class LeaderboardHistoryTask : IScheduledTask
	{
		public static DateTime LastUpdated { get; private set; }

		private readonly IHostingEnvironment _env;

		public string Schedule => "0 0 * * *";

		public LeaderboardHistoryTask(IHostingEnvironment env)
		{
			_env = env;
		}

		public async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			LastUpdated = DateTime.Now;

			FileResult file = await new GetLeaderboardModel().OnGetAsync();
			File.WriteAllBytes(Path.Combine(_env.WebRootPath, "leaderboard-history", file.FileDownloadName), ((FileContentResult)file).FileContents);
		}
	}
}