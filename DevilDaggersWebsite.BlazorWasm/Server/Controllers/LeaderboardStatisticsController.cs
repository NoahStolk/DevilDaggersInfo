﻿using DevilDaggersWebsite.Api.Attributes;
using DevilDaggersWebsite.BlazorWasm.Shared.Dto.LeaderboardStatistics;
using DevilDaggersWebsite.Caches.LeaderboardStatistics;
using DevilDaggersWebsite.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DevilDaggersWebsite.BlazorWasm.Server.Controllers
{
	[Route("api/leaderboard-statistics")]
	[ApiController]
	public class LeaderboardStatisticsController : ControllerBase
	{
		private readonly LeaderboardStatisticsCache _leaderboardStatisticsCache;

		public LeaderboardStatisticsController(LeaderboardStatisticsCache leaderboardStatisticsCache)
		{
			_leaderboardStatisticsCache = leaderboardStatisticsCache;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[EndpointConsumer(EndpointConsumers.Website)]
		public ActionResult<GetLeaderboardStatistics> GetStatistics()
		{
			return new GetLeaderboardStatistics
			{
				DaggerStatistics = _leaderboardStatisticsCache.DaggerStats.OrderBy(kvp => kvp.Key.UnlockSecond).ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value),
				DeathStatistics = _leaderboardStatisticsCache.DeathStats.OrderBy(kvp => kvp.Key.DeathType).ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value),
				TimeStatistics = _leaderboardStatisticsCache.TimeStats,
				DateTime = HistoryUtils.HistoryJsonFileNameToDateTime(_leaderboardStatisticsCache.FileName),
				IsFetched = _leaderboardStatisticsCache.IsFetched,
				AverageGems = _leaderboardStatisticsCache.AverageGems,
				AverageKills = _leaderboardStatisticsCache.AverageKills,
				AverageTimeInTenthsOfMilliseconds = _leaderboardStatisticsCache.AverageTimeInTenthsOfMilliseconds,
				EnemyStatistics = _leaderboardStatisticsCache.EnemyStats.OrderBy(kvp => kvp.Value).ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value),
				TotalEntries = _leaderboardStatisticsCache.Entries.Count,
			};
		}
	}
}
