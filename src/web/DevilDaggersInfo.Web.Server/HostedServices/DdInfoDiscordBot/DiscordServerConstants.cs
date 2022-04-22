using DSharpPlus;
using DSharpPlus.Entities;

namespace DevilDaggersInfo.Web.Server.HostedServices.DdInfoDiscordBot;

public static class DiscordServerConstants
{
	private const ulong _backgroundServiceMessageId = 856557628816490537;
	private const ulong _cacheMessageId = 856151636368031785;
	private const ulong _databaseMessageId = 856491602675236894;
	private const ulong _fileMessageId = 856265036486148146;

	public const long TestChannelId = 813508325705515008;

	public const ulong BotUserId = 645209987949395969;

	private static readonly Dictionary<Channel, ChannelWrapper> _channels = new()
	{
		{ Channel.MaintainersAuditLog, new(821489129615130684) },
		{ Channel.MonitoringBackgroundService, new(856553635206266911) },
		{ Channel.MonitoringCache, new(831981757200859206) },
		{ Channel.MonitoringCustomLeaderboardValid, new(813506112670007306) },
		{ Channel.MonitoringCustomLeaderboardInvalid, new(952210186859339816) },
		{ Channel.MonitoringDatabase, new(856490882371289098) },
		{ Channel.MonitoringFile, new(856263861372321823) },
		{ Channel.MonitoringLog, new(727227801664618607) },
		{ Channel.MonitoringTest, new(TestChannelId) },
		{ Channel.CustomLeaderboards, new(578316107836817418) },
	};

	public static DiscordMessage? BackgroundServiceMessage { get; private set; }
	public static DiscordMessage? CacheMessage { get; private set; }
	public static DiscordMessage? DatabaseMessage { get; private set; }
	public static DiscordMessage? FileMessage { get; private set; }

	/// <summary>
	/// Returns the channel based on the <paramref name="channel"/> enum. Always returns the channel for <see cref="Channel.MonitoringTest"/> when running in development.
	/// </summary>
	public static DiscordChannel? GetDiscordChannel(Channel channel, IWebHostEnvironment environment)
	{
		if (environment.IsDevelopment())
			channel = Channel.MonitoringTest;

		return _channels[channel].DiscordChannel;
	}

	public static async Task LoadServerChannelsAndMessages(DiscordClient client)
	{
		foreach (ChannelWrapper wrapper in _channels.Values)
		{
			if (wrapper.DiscordChannel == null)
				wrapper.DiscordChannel = await client.GetChannelAsync(wrapper.ChannelId);
		}

		DiscordChannel? backgroundServiceChannel = _channels[Channel.MonitoringBackgroundService].DiscordChannel;
		if (backgroundServiceChannel != null)
			BackgroundServiceMessage = await backgroundServiceChannel.GetMessageAsync(_backgroundServiceMessageId);

		DiscordChannel? cacheChannel = _channels[Channel.MonitoringCache].DiscordChannel;
		if (cacheChannel != null)
			CacheMessage = await cacheChannel.GetMessageAsync(_cacheMessageId);

		DiscordChannel? databaseChannel = _channels[Channel.MonitoringDatabase].DiscordChannel;
		if (databaseChannel != null)
			DatabaseMessage = await databaseChannel.GetMessageAsync(_databaseMessageId);

		DiscordChannel? fileChannel = _channels[Channel.MonitoringFile].DiscordChannel;
		if (fileChannel != null)
			FileMessage = await fileChannel.GetMessageAsync(_fileMessageId);
	}

	private sealed class ChannelWrapper
	{
		public ChannelWrapper(ulong channelId)
		{
			ChannelId = channelId;
		}

		public ulong ChannelId { get; }

		public DiscordChannel? DiscordChannel { get; set; }
	}
}
