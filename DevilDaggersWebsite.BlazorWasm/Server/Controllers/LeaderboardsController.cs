﻿using DevilDaggersWebsite.BlazorWasm.Server.Clients.Leaderboard;
using DevilDaggersWebsite.BlazorWasm.Server.Controllers.Attributes;
using DevilDaggersWebsite.BlazorWasm.Server.Converters;
using DevilDaggersWebsite.BlazorWasm.Shared.Dto.Leaderboards;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.BlazorWasm.Server.Controllers
{
	[Route("api/leaderboards")]
	[ApiController]
	public class LeaderboardsController : ControllerBase
	{
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[EndpointConsumer(EndpointConsumers.Website)]
		public async Task<ActionResult<GetLeaderboardPublic?>> GetLeaderboard([Range(1, int.MaxValue)] int rankStart = 1)
		{
			LeaderboardResponse l = await LeaderboardClient.Instance.GetScores(rankStart);
			return l.ToGetLeaderboardPublic();
		}

		[HttpGet("user/by-id")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[EndpointConsumer(EndpointConsumers.Website)]
		public async Task<ActionResult<GetEntryPublic>> GetPlayerById([Required, Range(1, int.MaxValue)] int userId)
		{
			EntryResponse e = await LeaderboardClient.Instance.GetUserById(userId);
			return e.ToGetEntryPublic();
		}

		[HttpGet("user/by-ids")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[EndpointConsumer(EndpointConsumers.Website)]
		public async Task<ActionResult<List<GetEntryPublic>>> GetPlayersByIds(string commaSeparatedUserIds)
		{
			IEnumerable<int> userIds = commaSeparatedUserIds.Split(',').Where(s => int.TryParse(s, out _)).Select(int.Parse);

			List<EntryResponse> el = await LeaderboardClient.Instance.GetUsersByIds(userIds);
			return el.ConvertAll(e => e.ToGetEntryPublic());
		}

		[HttpGet("user/by-username")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[EndpointConsumer(EndpointConsumers.Website)]
		public async Task<ActionResult<List<GetEntryPublic>>> GetPlayersByName([Required, MinLength(3)] string username)
		{
			List<EntryResponse> el = await LeaderboardClient.Instance.GetUserSearch(username);
			return el.ConvertAll(e => e.ToGetEntryPublic());
		}

		[HttpGet("user/by-rank")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[EndpointConsumer(EndpointConsumers.Website)]
		public async Task<ActionResult<GetEntryPublic>> GetPlayerByRank([Required, Range(1, int.MaxValue)] int rank)
		{
			LeaderboardResponse l = await LeaderboardClient.Instance.GetScores(rank);
			if (l.Entries.Count == 0)
				return NotFound();

			return l.Entries[0].ToGetEntryPublic();
		}
	}
}
