﻿using DevilDaggersWebsite.BlazorWasm.Server.Caches;
using DevilDaggersWebsite.BlazorWasm.Server.Caches.LeaderboardHistory;
using DevilDaggersWebsite.BlazorWasm.Server.Caches.LeaderboardStatistics;
using DevilDaggersWebsite.BlazorWasm.Server.Caches.ModArchive;
using DevilDaggersWebsite.BlazorWasm.Server.Caches.SpawnsetData;
using DevilDaggersWebsite.BlazorWasm.Server.Caches.SpawnsetHash;
using DevilDaggersWebsite.BlazorWasm.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.BlazorWasm.Server.Controllers
{
	[Route("api/admin/caches")]
	[Authorize(Roles = Roles.Admin)]
	[ApiController]
	public class CachesController : ControllerBase
	{
		private readonly LeaderboardStatisticsCache _leaderboardStatisticsCache;
		private readonly LeaderboardHistoryCache _leaderboardHistoryCache;
		private readonly ModArchiveCache _modArchiveCache;
		private readonly SpawnsetDataCache _spawnsetDataCache;
		private readonly SpawnsetHashCache _spawnsetHashCache;

		public CachesController(
			LeaderboardStatisticsCache leaderboardStatisticsCache,
			LeaderboardHistoryCache leaderboardHistoryCache,
			ModArchiveCache modArchiveCache,
			SpawnsetDataCache spawnsetDataCache,
			SpawnsetHashCache spawnsetHashCache)
		{
			_leaderboardStatisticsCache = leaderboardStatisticsCache;
			_leaderboardHistoryCache = leaderboardHistoryCache;
			_modArchiveCache = modArchiveCache;
			_spawnsetDataCache = spawnsetDataCache;
			_spawnsetHashCache = spawnsetHashCache;
		}

		[HttpPost("clear")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult> ClearCache(CacheType cacheType)
		{
			switch (cacheType)
			{
				case CacheType.LeaderboardStatistics: await _leaderboardStatisticsCache.Initiate(); break;
				case CacheType.LeaderboardHistory: _leaderboardHistoryCache.Clear(); break;
				case CacheType.ModArchive: _modArchiveCache.Clear(); break;
				case CacheType.SpawnsetData: _spawnsetDataCache.Clear(); break;
				case CacheType.SpawnsetHash: _spawnsetHashCache.Clear(); break;
			}

			return Ok();
		}
	}
}
