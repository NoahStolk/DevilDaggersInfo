using System.Numerics;

namespace DevilDaggersInfo.Core.Spawnset;

public class SpawnsetBinary
{
	public const int HeaderBufferSize = 36;
	public const int ArenaBufferSize = ArenaWidth * ArenaHeight * sizeof(float);
	public const int SpawnBufferSize = 28;

	public const int ArenaWidth = 51;
	public const int ArenaHeight = 51;

	public SpawnsetBinary(
		int spawnVersion,
		int worldVersion,
		float shrinkStart,
		float shrinkEnd,
		float shrinkRate,
		float brightness,
		GameMode gameMode,
		Vector2 raceDaggerPosition,
		float[,] arenaTiles,
		Spawn[] spawns,
		HandLevel handLevel,
		int additionalGems,
		float timerStart)
	{
		if (arenaTiles.GetLength(0) != ArenaWidth || arenaTiles.GetLength(1) != ArenaHeight)
			throw new ArgumentOutOfRangeException(nameof(arenaTiles), $"Arena array must be {ArenaWidth} by {ArenaHeight}.");

		SpawnVersion = spawnVersion;
		WorldVersion = worldVersion;
		ShrinkStart = shrinkStart;
		ShrinkEnd = shrinkEnd;
		ShrinkRate = shrinkRate;
		Brightness = brightness;
		GameMode = gameMode;
		RaceDaggerPosition = raceDaggerPosition;
		ArenaTiles = arenaTiles;
		Spawns = spawns;
		HandLevel = handLevel;
		AdditionalGems = additionalGems;
		TimerStart = timerStart;
	}

	public int SpawnVersion { get; }
	public int WorldVersion { get; }
	public float ShrinkStart { get; }
	public float ShrinkEnd { get; }
	public float ShrinkRate { get; }
	public float Brightness { get; }
	public GameMode GameMode { get; }
	public Vector2 RaceDaggerPosition { get; }

	public float[,] ArenaTiles { get; }
	public Spawn[] Spawns { get; }

	public HandLevel HandLevel { get; }
	public int AdditionalGems { get; }
	public float TimerStart { get; }

	#region Parsing

	public static bool TryParse(byte[] fileContents, [NotNullWhen(true)] out SpawnsetBinary? spawnsetBinary)
	{
		try
		{
			spawnsetBinary = Parse(fileContents);
			return true;
		}
		catch
		{
			// TODO: Log exceptions.
			spawnsetBinary = null;
			return false;
		}
	}

	// TODO: Throw clear exceptions when parsing fails.
	public static SpawnsetBinary Parse(byte[] fileContents)
	{
		using MemoryStream ms = new(fileContents);
		using BinaryReader br = new(ms);
		br.BaseStream.Position = 0;

		// Header
		int spawnVersion = br.ReadInt32();
		int worldVersion = br.ReadInt32();
		float shrinkEnd = br.ReadSingle();
		float shrinkStart = br.ReadSingle();
		float shrinkRate = br.ReadSingle();
		float brightness = br.ReadSingle();
		GameMode gameMode = br.ReadInt32().ToGameMode();
		br.Seek(8);

		// Arena
		float[,] arenaTiles = new float[ArenaWidth, ArenaHeight];
		for (int i = 0; i < ArenaWidth * ArenaHeight; i++)
		{
			int x = i % ArenaHeight;
			int y = i / ArenaWidth;
			arenaTiles[x, y] = br.ReadSingle();
		}

		// Spawns header
		float raceDaggerX = br.ReadSingle();
		float raceDaggerZ = br.ReadSingle();
		br.Seek(worldVersion >= 9 ? 28 : 24);
		int spawnCount = br.ReadInt32();

		// Spawns
		Spawn[] spawns = new Spawn[spawnCount];
		for (int i = 0; i < spawnCount; i++)
		{
			EnemyType enemyType = br.ReadInt32().ToEnemyType();
			float delay = br.ReadSingle();
			spawns[i] = new(enemyType, delay);

			br.Seek(20);
		}

		// Settings
		HandLevel handLevel = HandLevel.Level1;
		int additionalGems = 0;
		float timerStart = 0;
		if (spawnVersion >= 5)
		{
			handLevel = br.ReadByte().ToHandLevel();
			additionalGems = br.ReadInt32();

			if (spawnVersion >= 6)
				timerStart = br.ReadSingle();
		}

		return new(spawnVersion, worldVersion, shrinkStart, shrinkEnd, shrinkRate, brightness, gameMode, new(raceDaggerX, raceDaggerZ), arenaTiles, spawns, handLevel, additionalGems, timerStart);
	}

	#endregion Parsing

	#region Converting

	public byte[] ToBytes()
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);

		// Header
		bw.Write(SpawnVersion);
		bw.Write(WorldVersion);
		bw.Write(ShrinkEnd);
		bw.Write(ShrinkStart);
		bw.Write(ShrinkRate);
		bw.Write(Brightness);
		bw.Write((int)GameMode);
		bw.Write(0x33);
		bw.Write(0x01);

		// Arena
		for (int i = 0; i < ArenaWidth * ArenaHeight; i++)
		{
			int x = i % ArenaHeight;
			int y = i / ArenaWidth;

			bw.Write(ArenaTiles[x, y]);
		}

		// Spawns header
		bw.Write(RaceDaggerPosition.X);
		bw.Write(RaceDaggerPosition.Y);
		bw.Seek(4, SeekOrigin.Current);
		bw.Write(0x01);
		bw.Write(WorldVersion == 8 ? (byte)0x90 : (byte)0xF4);
		bw.Write((byte)0x01);
		bw.Seek(2, SeekOrigin.Current);
		bw.Write(0xFA);
		bw.Write(0x78);
		bw.Write(0x3C);
		if (WorldVersion >= 9)
			bw.Seek(4, SeekOrigin.Current);
		bw.Write(Spawns.Length);

		// Spawns
		foreach (Spawn spawn in Spawns)
		{
			bw.Write((int)spawn.EnemyType);
			bw.Write(spawn.Delay);
			bw.Seek(4, SeekOrigin.Current);
			bw.Write(0x03);
			bw.Seek(6, SeekOrigin.Current);
			bw.Write((byte)0xF0);
			bw.Write((byte)0x41);
			bw.Write(0x0A);
		}

		// Settings
		if (SpawnVersion >= 5)
		{
			bw.Write((byte)HandLevel);
			bw.Write(AdditionalGems);
			if (SpawnVersion >= 6)
				bw.Write(TimerStart);
		}

		return ms.ToArray();
	}

	#endregion Converting

	#region Utilities

	public static SpawnsetBinary CreateDefault()
		=> new(6, 9, 50, 20, 0.025f, 60, GameMode.Default, default, new float[ArenaWidth, ArenaHeight], Array.Empty<Spawn>(), HandLevel.Level1, 0, 0);

	public static bool IsEmptySpawn(int enemyType)
		=> enemyType < 0 || enemyType > 9;

	public bool HasSpawns()
		=> HasSpawns(Spawns);

	public static bool HasSpawns(Spawn[] spawns)
		=> spawns.Any(s => s.EnemyType != EnemyType.Empty);

	public bool HasEndLoop()
		=> HasEndLoop(Spawns, GameMode);

	public static bool HasEndLoop(Spawn[] spawns, GameMode gameMode)
	{
		if (gameMode != GameMode.Default || !HasSpawns(spawns))
			return false;

		int loopStartIndex = GetLoopStartIndex(spawns);
		Spawn[] loopSpawns = spawns.Skip(loopStartIndex).ToArray();

		return loopSpawns.Any(s => s.EnemyType != EnemyType.Empty);
	}

	public int GetLoopStartIndex()
		=> GetLoopStartIndex(Spawns);

	public static int GetLoopStartIndex(Spawn[] spawns)
	{
		for (int i = spawns.Length - 1; i >= 0; i--)
		{
			if (spawns[i].EnemyType == EnemyType.Empty)
				return i;
		}

		return 0;
	}

	public IEnumerable<double> GenerateEndWaveTimes(double endGameSecond, int waveIndex)
		=> GenerateEndWaveTimes(Spawns, endGameSecond, waveIndex);

	public static IEnumerable<double> GenerateEndWaveTimes(Spawn[] spawns, double endGameSecond, int waveIndex)
	{
		double enemyTimer = 0;
		double delay = 0;

		foreach (Spawn spawn in spawns.Skip(GetLoopStartIndex(spawns)))
		{
			delay += spawn.Delay;
			while (enemyTimer < delay)
			{
				endGameSecond += 1f / 60f;
				enemyTimer += 1f / 60f + 1f / 60f / 8f * waveIndex;
			}

			yield return endGameSecond;
		}
	}

	public EffectivePlayerSettings GetEffectivePlayerSettings()
		=> GetEffectivePlayerSettings(HandLevel, AdditionalGems);

	public static EffectivePlayerSettings GetEffectivePlayerSettings(HandLevel handLevel, int additionalGems)
	{
		if (handLevel == HandLevel.Level1)
		{
			if (additionalGems < 10)
				return new(HandLevel.Level1, additionalGems, HandLevel.Level1);

			if (additionalGems < 70)
				return new(HandLevel.Level2, additionalGems, HandLevel.Level2);

			if (additionalGems == 70)
				return new(HandLevel.Level3, 0, HandLevel.Level3);

			if (additionalGems == 71)
				return new(HandLevel.Level4, 0, HandLevel.Level4);

			return new(HandLevel.Level4, 0, HandLevel.Level3);
		}

		if (handLevel == HandLevel.Level2)
		{
			if (additionalGems < 0)
				return new(HandLevel.Level1, additionalGems + 10, HandLevel.Level1);

			return new(HandLevel.Level2, Math.Min(59, additionalGems) + 10, HandLevel.Level2);
		}

		if (handLevel == HandLevel.Level3)
			return new(HandLevel.Level3, Math.Min(149, additionalGems), HandLevel.Level3);

		return new(HandLevel.Level4, additionalGems, HandLevel.Level4);
	}

	public string GetGameVersionString()
		=> GetGameVersionString(WorldVersion, SpawnVersion);

	public static string GetGameVersionString(int worldVersion, int spawnVersion)
		=> worldVersion == 8 ? "V0 / V1" : spawnVersion == 4 ? "V2 / V3" : "V3.1 / V3.2";

	#endregion Utilities
}
