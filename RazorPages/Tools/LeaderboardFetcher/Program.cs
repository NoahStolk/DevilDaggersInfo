﻿using LeaderboardFetcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
#if TASK_WHEN_ALL
using System.Linq;
#endif
using System.Net.Http;
using System.Threading.Tasks;

const int _totalPages = 2424; // TODO: Fetch the leaderboard one more time on startup to retrieve total amount of players.

#if TASK_WHEN_ALL
Task<List<CompressedEntry>>[] tasks = new Task<List<CompressedEntry>>[_totalPages];
for (int i = 0; i < _totalPages; i++)
	tasks[i] = Fetch(i * 100 + 1);

Stopwatch stopwatch = new();
stopwatch.Start();
List<CompressedEntry> allEntries = (await Task.WhenAll(tasks)).SelectMany(e => e).ToList();
Console.WriteLine($"\nFetching {_totalPages} pages took {stopwatch.ElapsedMilliseconds / 1000f} seconds.");

File.WriteAllBytes($"{DateTime.UtcNow}.bin", GetBytes(allEntries));
#else
string start = DateTime.UtcNow.ToString("yyyyMMddHHmm");
Stopwatch stopwatch = new();
List<CompressedEntry> entries = new();
for (int i = 0; i < _totalPages; i++)
{
	stopwatch.Restart();
	entries.AddRange(await Fetch(i * 100 + 1));
	Console.WriteLine($"Fetching page {i + 1}/{_totalPages} took {stopwatch.ElapsedMilliseconds / 1000f} seconds.");

	// Write the file after every fetch in case it crashes and all progress is lost.
	File.WriteAllBytes($"{start}.bin", GetBytes(entries));
}
#endif

static byte[] GetBytes(List<CompressedEntry> entries)
{
	byte[] bytes = new byte[entries.Count * 15];
	for (int j = 0; j < entries.Count; j++)
		Buffer.BlockCopy(entries[j].ToBytes(), 0, bytes, j * 15, 15);
	return bytes;
}

static async Task<List<CompressedEntry>> Fetch(int rank)
{
	List<KeyValuePair<string?, string?>> postValues = new()
	{
		new("user", "0"),
		new("level", "survival"),
		new("offset", (rank - 1).ToString(CultureInfo.InvariantCulture)),
	};

	using FormUrlEncodedContent content = new(postValues);
	using HttpClient client = new();
	HttpResponseMessage response = await client.PostAsync("http://dd.hasmodai.com/backend15/get_scores.php", content);
	byte[] data = await response.Content.ReadAsByteArrayAsync();

	int entryCount = BitConverter.ToInt16(data, 59);
	int rankIterator = 0;
	int bytePos = 83;

	List<CompressedEntry> compressedEntries = new();
	while (rankIterator < entryCount)
	{
		short usernameLength = BitConverter.ToInt16(data, bytePos);
		bytePos += 2 + usernameLength;

		CompressedEntry entry = new()
		{
			Time = (uint)BitConverter.ToInt32(data, bytePos + 8),
			Kills = (ushort)BitConverter.ToInt32(data, bytePos + 12),
			Gems = (ushort)BitConverter.ToInt32(data, bytePos + 24),
			DaggersHit = (ushort)BitConverter.ToInt32(data, bytePos + 20),
			DaggersFired = (uint)BitConverter.ToInt32(data, bytePos + 16),
			DeathType = (byte)BitConverter.ToInt16(data, bytePos + 28),
		};
		compressedEntries.Add(entry);

		bytePos += 84;
		rankIterator++;
	}

	Console.WriteLine($"Fetched rank {rank} - {rank + 99}.");

	return compressedEntries;
}