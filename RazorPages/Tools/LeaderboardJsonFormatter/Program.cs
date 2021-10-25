﻿using DevilDaggersWebsite.Dto;
using Newtonsoft.Json;
using System;
using System.IO;

DateTime fullHistoryDateStart = new(2018, 10, 1);

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable S1075 // URIs should not be hardcoded
foreach (string path in Directory.GetFiles(@"C:\Users\NOAH\source\repos\DevilDaggersWebsite\DevilDaggersWebsite.Razor\wwwroot\leaderboard-history", "*.json"))
#pragma warning restore S1075 // URIs should not be hardcoded
#pragma warning restore IDE0079 // Remove unnecessary suppression
{
	try
	{
		Leaderboard leaderboard = JsonConvert.DeserializeObject<Leaderboard>(File.ReadAllText(path)) ?? throw new("Could not deserialize leaderboard.");
		Formatting formatting = leaderboard.DateTime > fullHistoryDateStart ? Formatting.None : Formatting.Indented;
		File.WriteAllText(path, JsonConvert.SerializeObject(leaderboard, formatting));

		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"SUCCESS for {path}: {formatting}");
	}
	catch (Exception ex)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine($"FAIL for {path}: {ex.Message}");
	}
}