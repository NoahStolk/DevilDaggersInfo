﻿using DevilDaggersCore.Extensions;
using DevilDaggersCore.Utils;
using DevilDaggersWebsite.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevilDaggersWebsite.Razor.Pages.CustomLeaderboards
{
	public class LeaderboardModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public LeaderboardModel(ApplicationDbContext dbContext)
		{
			_context = dbContext;
		}

		public SpawnsetFile? SpawnsetFile { get; private set; }

		[BindProperty]
		public CustomLeaderboard? Leaderboard { get; set; }

		[BindProperty]
		public List<CustomEntry>? Entries { get; private set; }

		public ActionResult? OnGet(string spawnsetName)
		{
			if (string.IsNullOrEmpty(spawnsetName))
				return RedirectToPage("Index");

			SpawnsetFile = _context.SpawnsetFiles.Include(sf => sf.Player).FirstOrDefault(sf => sf.Name == spawnsetName);
			if (SpawnsetFile == null)
				return RedirectToPage("Index");

			Leaderboard = _context.CustomLeaderboards.FirstOrDefault(l => l.SpawnsetFileId == SpawnsetFile.Id);
			if (Leaderboard == null)
				return RedirectToPage("Index");

			Entries = _context.CustomEntries
				.Where(e => e.CustomLeaderboard == Leaderboard)
				.OrderByMember(nameof(CustomEntry.Time), Leaderboard.IsAscending())
				.ThenByMember(nameof(CustomEntry.SubmitDate), true)
				.ToList();

			return null;
		}

		public (string DaggerName, string Seconds) GetDaggerInfo(int daggerIndex)
		{
			if (daggerIndex < 0 || daggerIndex > 4)
				throw new ArgumentOutOfRangeException(nameof(daggerIndex), $"'{nameof(daggerIndex)}' must be between 0 (Bronze) and 4 (Leviathan). '{nameof(daggerIndex)}' was {daggerIndex}.");

			string daggerName = daggerIndex switch
			{
				0 => "Bronze",
				1 => "Silver",
				2 => "Golden",
				3 => "Devil",
				_ => "Leviathan",
			};
			string? seconds = daggerIndex switch
			{
				0 => Leaderboard!.TimeBronze.FormatTimeInteger(),
				1 => Leaderboard!.TimeSilver.FormatTimeInteger(),
				2 => Leaderboard!.TimeGolden.FormatTimeInteger(),
				3 => Leaderboard!.TimeDevil.FormatTimeInteger(),
				_ => Leaderboard!.TimeLeviathan.FormatTimeInteger(),
			};

			return (daggerName, seconds ?? "?");
		}
	}
}
