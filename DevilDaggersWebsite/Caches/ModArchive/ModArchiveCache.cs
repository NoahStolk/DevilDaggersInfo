﻿using DevilDaggersDiscordBot.Logging;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace DevilDaggersWebsite.Caches.Mod
{
	public sealed class ModArchiveCache
	{
		private readonly ConcurrentDictionary<string, ModArchiveCacheData> _cache = new();

		private static readonly Lazy<ModArchiveCache> _lazy = new(() => new());

		private ModArchiveCache()
		{
		}

		public static ModArchiveCache Instance => _lazy.Value;

		public ModArchiveCacheData GetArchiveDataByFilePath(string filePath)
		{
			string name = Path.GetFileNameWithoutExtension(filePath);
			if (_cache.ContainsKey(name))
				return _cache[name];

			using FileStream fs = new(filePath, FileMode.Open);
			using ZipArchive archive = new(fs);
			ModArchiveCacheData archiveData = new()
			{
				FileSize = fs.Length,
			};
			foreach (ZipArchiveEntry entry in archive.Entries)
			{
				byte[] extractedContents = new byte[entry.Length];

				using Stream stream = entry.Open();
				stream.Read(extractedContents, 0, extractedContents.Length);

				archiveData.ModData.Add(Dto.ModData.CreateFromFile(entry.Name, extractedContents));
				archiveData.FileSizeExtracted += entry.Length;
			}

			_cache.TryAdd(name, archiveData);
			return archiveData;
		}

		public async Task Clear(IWebHostEnvironment env)
		{
			int cacheCount = _cache.Count;
			_cache.Clear();
			await DiscordLogger.Instance.TryLog(Channel.CacheMonitoring, env.EnvironmentName, $"Successfully cleared `{nameof(ModArchiveCache)}`. (Removed `{cacheCount}` instances.)");
		}
	}
}