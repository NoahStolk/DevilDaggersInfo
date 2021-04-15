﻿using DevilDaggersCore.Mods;
using DevilDaggersWebsite.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevilDaggersWebsite.Dto
{
	public class ModData
	{
		public static readonly ulong Magic1 = MakeMagic(0x3AUL, 0x68UL, 0x78UL, 0x3AUL);
		public static readonly ulong Magic2 = MakeMagic(0x72UL, 0x67UL, 0x3AUL, 0x01UL);

		public ModData(string name, ModBinaryType modBinaryType, List<ModAssetData> modAssetData)
		{
			Name = name;
			ModBinaryType = modBinaryType;
			ModAssetData = modAssetData;
		}

		public string Name { get; }
		public ModBinaryType ModBinaryType { get; }
		public List<ModAssetData> ModAssetData { get; }

		private static ulong MakeMagic(ulong a, ulong b, ulong c, ulong d)
			=> a | b << 8 | c << 16 | d << 24;

		public static ModData CreateFromFile(string fileName, byte[] fileContents)
		{
			ModBinaryType modBinaryType;
			if (fileName.StartsWith("audio"))
				modBinaryType = ModBinaryType.Audio;
			else if (fileName.StartsWith("core"))
				modBinaryType = ModBinaryType.Core;
			else if (fileName.StartsWith("dd"))
				modBinaryType = ModBinaryType.Dd;
			else
				throw new InvalidModBinaryException($"File '{fileName}' must start with 'audio', 'core', or 'dd'.");

			uint magic1FromFile = BitConverter.ToUInt32(fileContents, 0);
			uint magic2FromFile = BitConverter.ToUInt32(fileContents, 4);
			if (magic1FromFile != Magic1 || magic2FromFile != Magic2)
				throw new InvalidModBinaryException($"File '{fileName}' is not a valid binary.");

			uint tocSize = BitConverter.ToUInt32(fileContents, 8);
			byte[] tocBuffer = new byte[tocSize];
			Buffer.BlockCopy(fileContents, 12, tocBuffer, 0, (int)tocSize);

			List<ModAssetData> chunks = new();
			int i = 0;
			while (i < tocBuffer.Length - 14)
			{
				byte type = tocBuffer[i];
				string name = ReadNullTerminatedString(tocBuffer, i + 2);
				i += name.Length + 15;
				AssetType assetType = type switch
				{
					0x01 => AssetType.Model,
					0x02 => AssetType.Texture,
					0x10 => AssetType.Shader,
					0x20 => AssetType.Audio,
					0x80 => AssetType.ModelBinding,
					_ => throw new InvalidModBinaryException($"File '{fileName}' contains an unknown asset type '{type}'. Valid types are {0x01}, {0x02}, {0x10}, {0x20}, and {0x80}."),
				};

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
						_ => throw new InvalidModBinaryException($"File '{fileName}', which is a '{modBinaryType}' binary file, contains an asset of type '{assetType}', which is not supported."),
					},
					_ => throw new InvalidModBinaryException($"Unknown binary type '{modBinaryType}'."),
				};

				chunks.Add(new(name, assetType, isProhibited));
			}

			return new(fileName, modBinaryType, chunks);

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

				throw new InvalidModBinaryException($"Null terminator not observed in buffer with length {buffer.Length} starting from offset {offset} in file '{fileName}'.");
			}
		}
	}
}