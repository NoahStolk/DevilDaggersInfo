﻿namespace DevilDaggersInfo.Core.Shared.Extensions;

public static class ByteArrayExtensions
{
	public static string ReadNullTerminatedString(this byte[] buffer, int offset)
		=> ReadNullTerminatedString(buffer, offset, Encoding.UTF8);

	public static string ReadNullTerminatedString(this byte[] buffer, int offset, Encoding encoding)
	{
		int indexOfNextNullTerminator = Array.IndexOf(buffer, (byte)0, offset);
		if (indexOfNextNullTerminator == -1)
			throw new ArgumentOutOfRangeException(nameof(offset), $"Null terminator not observed in buffer with length '{buffer.Length}' starting from offset '{offset}'.");

		return encoding.GetString(buffer[offset..indexOfNextNullTerminator]);
	}
}
