﻿using DevilDaggersWebsite.Models.API;
using DevilDaggersWebsite.PageModels;
using DevilDaggersWebsite.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Pages.API
{
	[ApiFunction(Description = "Returns 100 leaderboard entries from the current leaderboard starting at the rank parameter. The default is rank 1.", ReturnType = MediaTypeNames.Application.Json)]
	public class GetLeaderboardModel : ApiPageModel
	{
		public async Task<FileResult> OnGetAsync(int rank = 1, bool formatted = false)
		{
			rank = Math.Max(1, rank);

			Models.Leaderboard.Leaderboard leaderboard = await LeaderboardUtils.LoadLeaderboard(rank);

			return JsonFile(leaderboard, leaderboard.DateTime.ToString("yyyyMMddHHmm"), formatted ? Formatting.Indented : Formatting.None);
		}
	}
}