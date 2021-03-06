using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.Spawnset;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace DevilDaggersInfo.Core.Replay;

public class LocalReplayBinaryHeader : IReplayBinaryHeader<LocalReplayBinaryHeader>
{
	// TODO: Use byte[] when C# 11 officially comes out and remove the byte[] field.
	private const string _identifier = "ddrpl.";
	private static readonly byte[] _identifierBytes = Encoding.UTF8.GetBytes(_identifier);

	public LocalReplayBinaryHeader(
		int version,
		long timestampSinceGameRelease,
		float time,
		float startTime,
		int daggersFired,
		int deathType,
		int gems,
		int daggersHit,
		int kills,
		int playerId,
		string username,
		byte[] spawnsetBuffer)
	{
		Version = version;
		TimestampSinceGameRelease = timestampSinceGameRelease;
		Time = time;
		StartTime = startTime;
		DaggersFired = daggersFired;
		DeathType = deathType;
		Gems = gems;
		DaggersHit = daggersHit;
		Kills = kills;
		PlayerId = playerId;
		Username = username;
		SpawnsetMd5 = MD5.HashData(spawnsetBuffer);
		SpawnsetBuffer = spawnsetBuffer;
	}

	public int Version { get; }
	public long TimestampSinceGameRelease { get; }
	public float Time { get; }
	public float StartTime { get; }
	public int DaggersFired { get; }
	public int DeathType { get; }
	public int Gems { get; }
	public int DaggersHit { get; }
	public int Kills { get; }
	public int PlayerId { get; }
	public string Username { get; }
	public byte[] SpawnsetMd5 { get; }
	public byte[] SpawnsetBuffer { get; }

	public static bool UsesLengthPrefixedEvents => true;

	public static int IdentifierLength => _identifier.Length;

	public static LocalReplayBinaryHeader CreateFromByteArray(byte[] contents)
	{
		using MemoryStream ms = new(contents);
		using BinaryReader br = new(ms);
		return CreateFromBinaryReader(br);
	}

	public static LocalReplayBinaryHeader CreateFromBinaryReader(BinaryReader br)
	{
		if (!IdentifierIsValid(br, out byte[]? identifier))
		{
			if (identifier == null)
				throw new InvalidReplayBinaryException("Local replay identifier could not be determined.");

			throw new InvalidReplayBinaryException($"'{Encoding.UTF8.GetString(identifier)}' / '{identifier.ByteArrayToHexString()}' is not a valid local replay identifier.");
		}

		int version = br.ReadInt32();
		long timestampSinceGameRelease = br.ReadInt64();
		float time = br.ReadSingle();
		float startTime = br.ReadSingle();
		int daggersFired = br.ReadInt32();
		int deathType = br.ReadInt32();
		int gems = br.ReadInt32();
		int daggersHit = br.ReadInt32();
		int kills = br.ReadInt32();
		int playerId = br.ReadInt32();
		int usernameLength = br.ReadInt32();
		byte[] usernameBytes = br.ReadBytes(usernameLength);
		string username = Encoding.UTF8.GetString(usernameBytes);
		br.BaseStream.Seek(10, SeekOrigin.Current);
		byte[] spawnsetMd5 = br.ReadBytes(16);
		int spawnsetLength = br.ReadInt32();
		byte[] spawnsetBuffer = br.ReadBytes(spawnsetLength);

		if (!ArrayUtils.AreEqual(spawnsetMd5, MD5.HashData(spawnsetBuffer)))
			throw new InvalidReplayBinaryException("Hashed spawnset data does not match the spawnset buffer.");

		return new(
			version: version,
			timestampSinceGameRelease: timestampSinceGameRelease,
			time: time,
			startTime: startTime,
			daggersFired: daggersFired,
			deathType: deathType,
			gems: gems,
			daggersHit: daggersHit,
			kills: kills,
			playerId: playerId,
			username: username,
			spawnsetBuffer: spawnsetBuffer);
	}

	public static bool IdentifierIsValid(byte[] contents, [MaybeNullWhen(false)] out byte[]? identifier)
	{
		using MemoryStream ms = new(contents);
		using BinaryReader br = new(ms);
		return IdentifierIsValid(br, out identifier);
	}

	public static bool IdentifierIsValid(BinaryReader br, [MaybeNullWhen(false)] out byte[]? identifier)
	{
		identifier = null;
		if (br.BaseStream.Position > br.BaseStream.Length - _identifier.Length)
			return false;

		identifier = br.ReadBytes(_identifier.Length);
		return ArrayUtils.AreEqual(_identifierBytes, identifier);
	}

	public static LocalReplayBinaryHeader CreateDefault()
	{
		SpawnsetBinary spawnset = SpawnsetBinary.CreateDefault();
		byte[] spawnsetBuffer = spawnset.ToBytes();
		return new(
			version: 1,
			timestampSinceGameRelease: 0, // TODO: Convert current time to timestamp.
			time: 0,
			startTime: 0,
			daggersFired: 0,
			deathType: 0,
			gems: 0,
			daggersHit: 0,
			kills: 0,
			playerId: 0,
			username: string.Empty,
			spawnsetBuffer: spawnsetBuffer);
	}

	public byte[] ToBytes()
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);

		bw.Write(Encoding.UTF8.GetBytes(_identifier));
		bw.Write(Version);
		bw.Write(TimestampSinceGameRelease);
		bw.Write(Time);
		bw.Write(StartTime);
		bw.Write(DaggersFired);
		bw.Write(DeathType);
		bw.Write(Gems);
		bw.Write(DaggersHit);
		bw.Write(Kills);
		bw.Write(PlayerId);
		bw.Write(Username.Length);
		bw.Write(Encoding.UTF8.GetBytes(Username));
		bw.Seek(10, SeekOrigin.Current);
		bw.Write(SpawnsetMd5);
		bw.Write(SpawnsetBuffer.Length);
		bw.Write(SpawnsetBuffer);

		return ms.ToArray();
	}
}
