﻿using DevilDaggersWebsite.Enumerators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevilDaggersWebsite.Dto
{
	public class UploadRequest
	{
		public byte[] SurvivalHashMd5 { get; init; } = null!;

		public int PlayerId { get; init; }

		[StringLength(32)]
		public string PlayerName { get; init; } = null!;

		public int Time { get; set; } // Use set to fix replay times.

		public int GemsCollected { get; init; }

		public int EnemiesKilled { get; init; }

		public int DaggersFired { get; init; }

		public int DaggersHit { get; init; }

		public int EnemiesAlive { get; init; }

		public int HomingDaggers { get; set; } // Use set to fix negative values.

		public int HomingDaggersEaten { get; init; }

		public int GemsDespawned { get; init; }

		public int GemsEaten { get; init; }

		public int GemsTotal { get; init; }

		public byte DeathType { get; init; }

		public int LevelUpTime2 { get; init; }

		public int LevelUpTime3 { get; init; }

		public int LevelUpTime4 { get; init; }

		[StringLength(16)]
		public string ClientVersion { get; init; } = null!;

		public CustomLeaderboardsClient Client { get; init; }

		public OperatingSystem OperatingSystem { get; init; }

		public BuildMode BuildMode { get; init; }

		public string Validation { get; set; } = null!; // Use set for unit tests.

		public bool IsReplay { get; init; }

		public bool ProhibitedMods { get; init; }

		public List<GameState> GameStates { get; init; } = new();
	}
}