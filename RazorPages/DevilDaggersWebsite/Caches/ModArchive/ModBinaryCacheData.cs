﻿using DevilDaggersCore.Mods;
using DevilDaggersWebsite.Enumerators;
using DevilDaggersWebsite.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevilDaggersWebsite.Caches.ModArchive
{
	public class ModBinaryCacheData
	{
		public static readonly ulong Magic1 = MakeMagic(0x3AUL, 0x68UL, 0x78UL, 0x3AUL);
		public static readonly ulong Magic2 = MakeMagic(0x72UL, 0x67UL, 0x3AUL, 0x01UL);

		public ModBinaryCacheData(string name, long size, ModBinaryType modBinaryType, List<ModChunkCacheData> chunks, List<(string Name, bool IsProhibited)>? loudnessAssets)
		{
			Name = name;
			Size = size;
			ModBinaryType = modBinaryType;
			Chunks = chunks;
			LoudnessAssets = loudnessAssets;
		}

		public string Name { get; }
		public long Size { get; }
		public ModBinaryType ModBinaryType { get; }
		public List<ModChunkCacheData> Chunks { get; }
		public List<(string Name, bool IsProhibited)>? LoudnessAssets { get; }

		private static ulong MakeMagic(ulong a, ulong b, ulong c, ulong d)
			=> a | b << 8 | c << 16 | d << 24;

		public static ModBinaryCacheData CreateFromFile(string fileName, byte[] fileContents)
		{
			ModBinaryType modBinaryType;
			if (fileName.StartsWith("audio"))
				modBinaryType = ModBinaryType.Audio;
			else if (fileName.StartsWith("core"))
				modBinaryType = ModBinaryType.Core;
			else if (fileName.StartsWith("dd"))
				modBinaryType = ModBinaryType.Dd;
			else
				throw new InvalidModBinaryException($"File `{fileName}` must start with `audio`, `core`, or `dd`.");

			uint magic1FromFile = BitConverter.ToUInt32(fileContents, 0);
			uint magic2FromFile = BitConverter.ToUInt32(fileContents, 4);
			if (magic1FromFile != Magic1 || magic2FromFile != Magic2)
				throw new InvalidModBinaryException($"File `{fileName}` is not a valid binary.");

			uint tocSize = BitConverter.ToUInt32(fileContents, 8);
			byte[] tocBuffer = new byte[tocSize];
			Buffer.BlockCopy(fileContents, 12, tocBuffer, 0, (int)tocSize);

			List<ModChunkCacheData> chunks = new();
			List<(string Name, bool IsProhibited)>? loudnessAssets = null;
			int i = 0;
			while (i < tocBuffer.Length - 14)
			{
				byte type = tocBuffer[i];
				string name = ReadNullTerminatedString(tocBuffer, i + 2);

				i += name.Length + 1; // + 1 to include null terminator.
				uint startOffset = BitConverter.ToUInt32(tocBuffer, i + 2);
				uint size = BitConverter.ToUInt32(tocBuffer, i + 6);
				i += 14;
				AssetType assetType = type switch
				{
					0x01 => AssetType.Model,
					0x02 => AssetType.Texture,
					0x10 => AssetType.Shader,
					0x20 => AssetType.Audio,
					0x80 => AssetType.ModelBinding,
					_ => throw new InvalidModBinaryException($"File `{fileName}` contains an unknown asset type `{type}`. Valid types are {0x01}, {0x02}, {0x10}, {0x20}, and {0x80}."),
				};

				if (assetType == AssetType.Audio && name == "loudness")
				{
					byte[] loudnessBytes = new byte[size];
					Buffer.BlockCopy(fileContents, (int)startOffset, loudnessBytes, 0, (int)size);
					string loudnessString = Encoding.Default.GetString(loudnessBytes);

					loudnessAssets = new();
					foreach (string line in loudnessString.Split('\n'))
					{
						if (!TryReadLoudnessLine(line, out string? assetName, out float loudness) || assetName == null)
							continue;

						AudioAssetData? audioAssetData = AssetHandler.Instance.AudioAudioAssets.Find(a => a.AssetName == assetName);
						if (audioAssetData == null || audioAssetData.DefaultLoudness == loudness)
							continue;

						loudnessAssets.Add((assetName, audioAssetData.IsProhibited));
					}
				}
				else
				{
					bool isProhibited = modBinaryType switch
					{
						ModBinaryType.Audio => AssetHandler.Instance.AudioAudioAssets.Find(a => a.AssetName == name)?.IsProhibited ?? false,
						ModBinaryType.Core => AssetHandler.Instance.CoreShadersAssets.Find(a => a.AssetName == name)?.IsProhibited ?? false,
						ModBinaryType.Dd => assetType switch
						{
							AssetType.ModelBinding => AssetHandler.Instance.DdModelBindingsAssets.Find(a => a.AssetName == name)?.IsProhibited ?? false,
							AssetType.Model => AssetHandler.Instance.DdModelsAssets.Find(a => a.AssetName == name)?.IsProhibited ?? false,
							AssetType.Shader => AssetHandler.Instance.DdShadersAssets.Find(a => a.AssetName == name)?.IsProhibited ?? false,
							AssetType.Texture => AssetHandler.Instance.DdTexturesAssets.Find(a => a.AssetName == name)?.IsProhibited ?? false,
							_ => throw new InvalidModBinaryException($"File `{fileName}`, which is a `{modBinaryType}` binary file, contains an asset of type `{assetType}`, which is not supported."),
						},
						_ => throw new InvalidModBinaryException($"Unknown binary type `{modBinaryType}`."),
					};

					chunks.Add(new(name, size, assetType, isProhibited));
				}
			}

			return new(fileName, fileContents.Length, modBinaryType, chunks, loudnessAssets);

			string ReadNullTerminatedString(byte[] buffer, int offset)
			{
				StringBuilder sb = new();
				for (int i = offset; i < buffer.Length; i++)
				{
					char c = (char)buffer[i];
					if (c == '\0')
						return sb.ToString();
					sb.Append(c);
				}

				throw new InvalidModBinaryException($"Null terminator not observed in buffer with length `{buffer.Length}` starting from offset `{offset}` in file `{fileName}`.");
			}

			static bool TryReadLoudnessLine(string line, out string? assetName, out float loudness)
			{
				try
				{
					line = line
						.Replace(" ", string.Empty, StringComparison.InvariantCulture) // Remove spaces to make things easier.
						.TrimEnd('.'); // Remove dots at the end of the line. (The original loudness file has one on line 154 for some reason...)

					int equalsIndex = line.IndexOf('=', StringComparison.InvariantCulture);

					assetName = line.Substring(0, equalsIndex);
					loudness = float.Parse(line.Substring(equalsIndex + 1, line.Length - assetName.Length - 1));
					return true;
				}
				catch
				{
					assetName = null;
					loudness = 0;
					return false;
				}
			}
		}
	}
}