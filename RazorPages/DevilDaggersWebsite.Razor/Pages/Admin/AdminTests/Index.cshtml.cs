﻿using DevilDaggersWebsite.Caches.LeaderboardHistory;
using DevilDaggersWebsite.Caches.LeaderboardStatistics;
using DevilDaggersWebsite.Caches.ModArchive;
using DevilDaggersWebsite.Caches.SpawnsetData;
using DevilDaggersWebsite.Caches.SpawnsetHash;
using DevilDaggersWebsite.HostedServices.DdInfoDiscordBot;
using DevilDaggersWebsite.Singletons;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Razor.Pages.Admin.AdminTests
{
	public class IndexModel : PageModel
	{
		private readonly DiscordLogger _discordLogger;
		private readonly LeaderboardStatisticsCache _leaderboardStatisticsCache;
		private readonly LeaderboardHistoryCache _leaderboardHistoryCache;
		private readonly ModArchiveCache _modArchiveCache;
		private readonly SpawnsetDataCache _spawnsetDataCache;
		private readonly SpawnsetHashCache _spawnsetHashCache;

		public IndexModel(
			DiscordLogger discordLogger,
			LeaderboardStatisticsCache leaderboardStatisticsCache,
			LeaderboardHistoryCache leaderboardHistoryCache,
			ModArchiveCache modArchiveCache,
			SpawnsetDataCache spawnsetDataCache,
			SpawnsetHashCache spawnsetHashCache)
		{
			_discordLogger = discordLogger;
			_leaderboardStatisticsCache = leaderboardStatisticsCache;
			_leaderboardHistoryCache = leaderboardHistoryCache;
			_modArchiveCache = modArchiveCache;
			_spawnsetDataCache = spawnsetDataCache;
			_spawnsetHashCache = spawnsetHashCache;
		}

		public async Task OnPostTestBotAsync()
			=> await _discordLogger.TryLog(Channel.MonitoringTest, "Hello, this is a test message sent from an external environment.");

		public void OnPostTestException()
			=> throw new("TEST EXCEPTION with 3 inner exceptions", new("Inner exception message", new("Another inner exception message", new("Big Discord embed"))));

		public async Task OnPostInitiateLeaderboardStatisticsCache()
			=> await _leaderboardStatisticsCache.Initiate();

		public void OnPostClearLeaderboardHistoryCache()
			=> _leaderboardHistoryCache.Clear();

		public void OnPostClearModDataCache()
			=> _modArchiveCache.Clear();

		public void OnPostClearSpawnsetDataCache()
			=> _spawnsetDataCache.Clear();

		public void OnPostClearSpawnsetHashCache()
			=> _spawnsetHashCache.Clear();

		public async Task OnPostTestColors()
		{
			for (int i = 0; i < 6; i++)
			{
				DiscordColor color = DiscordColors.Default;
				int time = i * 10000;
				if (time >= 50000)
					color = DiscordColors.Leviathan;
				else if (time >= 40000)
					color = DiscordColors.Devil;
				else if (time >= 30000)
					color = DiscordColors.Golden;
				else if (time >= 20000)
					color = DiscordColors.Silver;
				else if (time >= 10000)
					color = DiscordColors.Bronze;

				DiscordEmbedBuilder builder = new()
				{
					Title = "Test colors",
					Color = color,
				};
				await _discordLogger.TryLog(Channel.MonitoringTest, null, builder.Build());
			}
		}
	}
}