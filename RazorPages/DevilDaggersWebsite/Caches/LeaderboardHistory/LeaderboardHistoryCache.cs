﻿using DevilDaggersWebsite.Dto;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace DevilDaggersWebsite.Caches.LeaderboardHistory
{
	public class LeaderboardHistoryCache : IDynamicCache
	{
		private readonly ConcurrentDictionary<string, Leaderboard> _cache = new();

		public Leaderboard GetLeaderboardHistoryByFilePath(string filePath)
		{
			string name = Path.GetFileNameWithoutExtension(filePath);
			if (_cache.ContainsKey(name))
				return _cache[name];

			Leaderboard lb = JsonConvert.DeserializeObject<Leaderboard?>(File.ReadAllText(filePath, Encoding.UTF8)) ?? throw new($"Corrupt leaderboard history file: {name}");
			_cache.TryAdd(name, lb);
			return lb;
		}

		public void Clear()
			=> _cache.Clear();

		public string LogState()
			=> $"`{_cache.Count}` in memory";
	}
}