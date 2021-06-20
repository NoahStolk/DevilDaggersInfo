﻿using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevilDaggersDiscordBot
{
	public static class ServerConstants
	{
		internal const ulong BotUserId = 645209987949395969;

		internal const ulong CacheMessageId = 856151636368031785;
		internal const ulong FileMessageId = 856265036486148146;

		private static readonly Dictionary<Channel, ChannelWrapper> _channels = new()
		{
			{ Channel.MonitoringAuditLog, new(821489129615130684) },
			{ Channel.MonitoringCache, new(831981757200859206) },
			{ Channel.MonitoringCustomLeaderboard, new(813506112670007306) },
			{ Channel.MonitoringError, new(727227801664618607) },
			{ Channel.MonitoringFile, new(856263861372321823) },
			{ Channel.MonitoringTask, new(813506034970787880) },
			{ Channel.MonitoringTest, new(813508325705515008) },
			{ Channel.CustomLeaderboards, new(578316107836817418) },
		};

		internal static IReadOnlyDictionary<Channel, ChannelWrapper> Channels => _channels;

		public static DiscordMessage? CacheMessage { get; private set; }
		public static DiscordMessage? FileMessage { get; private set; }

		internal static async Task LoadServerChannelsAndMessages(DiscordClient client)
		{
			foreach (KeyValuePair<Channel, ChannelWrapper> kvp in _channels)
			{
				if (kvp.Value.DiscordChannel == null)
					kvp.Value.DiscordChannel = await client.GetChannelAsync(kvp.Value.ChannelId);
			}

			DiscordChannel? cacheChannel = _channels[Channel.MonitoringCache].DiscordChannel;
			if (cacheChannel != null)
				CacheMessage = await cacheChannel.GetMessageAsync(CacheMessageId);

			DiscordChannel? fileChannel = _channels[Channel.MonitoringFile].DiscordChannel;
			if (fileChannel != null)
				FileMessage = await fileChannel.GetMessageAsync(FileMessageId);
		}

		internal class ChannelWrapper
		{
			internal ChannelWrapper(ulong channelId)
			{
				ChannelId = channelId;
			}

			internal ulong ChannelId { get; }

			internal DiscordChannel? DiscordChannel { get; set; }
		}
	}
}
