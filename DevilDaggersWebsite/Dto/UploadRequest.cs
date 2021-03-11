﻿using DevilDaggersWebsite.Enumerators;
using System.Collections.Generic;

namespace DevilDaggersWebsite.Dto
{
	public class UploadRequest
	{
		public byte[] SurvivalHashMd5 { get; init; } = null!;
		public int PlayerId { get; init; }
		public string PlayerName { get; init; } = null!;
		public int Time { get; set; }
		public int GemsCollected { get; init; }
		public int EnemiesKilled { get; init; }
		public int DaggersFired { get; init; }
		public int DaggersHit { get; init; }
		public int EnemiesAlive { get; init; }
		public int HomingDaggers { get; init; }
		public int HomingDaggersEaten { get; init; }
		public int GemsDespawned { get; init; }
		public int GemsEaten { get; init; }
		public int GemsTotal { get; init; }
		public byte DeathType { get; init; }
		public int LevelUpTime2 { get; init; }
		public int LevelUpTime3 { get; init; }
		public int LevelUpTime4 { get; init; }
		public string ClientVersion { get; init; } = null!;
		public OperatingSystem OperatingSystem { get; init; }
		public BuildMode BuildMode { get; init; }
		public string Validation { get; set; } = null!;
		public bool IsReplay { get; set; }
		public List<GameState> GameStates { get; init; } = new();
	}
}
