﻿using DevilDaggersCore.Leaderboard;
using DevilDaggersWebsite.Code.API;
using DevilDaggersWebsite.Code.PageModels;
using DevilDaggersWebsite.Code.Utils.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Pages.API
{
	[ApiFunction(Description = "Returns the user data for the users containing the username search parameter.", ReturnType = MediaTypeNames.Application.Json)]
	public class GetUserByUsernameModel : ApiPageModel
	{
		public async Task<FileResult> OnGetAsync(string username, bool formatted = false)
		{
			List<Entry> leaderboard = await GetUserSearch(username);
			return JsonFile(leaderboard, formatted ? Formatting.Indented : Formatting.None);
		}

		public async Task<List<Entry>> GetUserSearch(string username)
		{
			DevilDaggersCore.Leaderboard.Leaderboard leaderboard = await Hasmodai.GetUserSearch(username);
			return leaderboard.Entries;
		}
	}
}